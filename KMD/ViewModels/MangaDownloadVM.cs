using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using KMD.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KMD.ViewModels
{
    public class MangaDownloadVM : ReactiveObject, IRoutableViewModel
    {
        private readonly Config _config = Config.Instance;
        private readonly List<Manga> _openedMangas = new List<Manga>();
        private SemaphoreSlim _currentDownloads;

        public MangaDownloadVM(IScreen screen)
        {
            HostScreen = screen;
            OpenUrl = ReactiveCommand.CreateFromTask(OpenUrlInternal, CanOpenUrl());
            SelectManga = ReactiveCommand.Create<string>(SelectMangaInternal);
            AddChaptersToDownload =
                ReactiveCommand.Create<IEnumerable<Chapter>>(AddChaptersToDownloadInternal, CanAddChaptersToDownload());
            RmChaptersFromDownload = ReactiveCommand.Create<IList>(RmChaptersFromDownloadInternal);
            OpenChapterFolder = ReactiveCommand.Create<IList>(OpenChapterFolderInternal);
            BeginDownload = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(BeginDownloadInternal).TakeUntil(CancelDownload),
                CanBeginDownload());
            CancelDownload = ReactiveCommand.Create(() => { }, BeginDownload.IsExecuting);
            OpenConfigWindow =
                ReactiveCommand.Create(OpenConfigWindowInternal, BeginDownload.IsExecuting.Select(x => !x));
            CurrentManga = null;
        }

        [Reactive] public Manga CurrentManga { get; private set; }

        [Reactive]
        public ObservableCollection<ChapterToDownload> ChaptersToDownload { get; set; } =
            new ObservableCollection<ChapterToDownload>();

        [Reactive] public string Status { get; private set; }

        [Reactive] public double Progress { get; set; }

        [Reactive] public string Url { get; set; }

        private bool NoNewDownloads =>
            ChaptersToDownload.Count < 1 ||
            ChaptersToDownload.All(chapter =>
                chapter.Status == ChapterToDownload.ChapterStatus.Error ||
                chapter.Status == ChapterToDownload.ChapterStatus.Incomplete ||
                chapter.Status == ChapterToDownload.ChapterStatus.Complete);

        private bool AnyNewDownload =>
            ChaptersToDownload.Any(ch => ch.Status == ChapterToDownload.ChapterStatus.ReadyToDownload);

        private ChapterToDownload ChapterReadyToDownload =>
            ChaptersToDownload.First(ch => ch.Status == ChapterToDownload.ChapterStatus.ReadyToDownload);

        public ReactiveCommand<Unit, Unit> OpenUrl { get; }
        public ReactiveCommand<string, Unit> SelectManga { get; }
        public ReactiveCommand<IEnumerable<Chapter>, Unit> AddChaptersToDownload { get; }
        public ReactiveCommand<IList, Unit> RmChaptersFromDownload { get; }
        public ReactiveCommand<IList, Unit> OpenChapterFolder { get; }
        public ReactiveCommand<Unit, Unit> BeginDownload { get; }
        public ReactiveCommand<Unit, Unit> CancelDownload { get; }
        public ReactiveCommand<Unit, Unit> OpenConfigWindow { get; }
        public IScreen HostScreen { get; }
        public string UrlPathSegment => "MangaDownload";

        private async Task OpenUrlInternal(CancellationToken ct)
        {
            if (!Url.StartsWith("http"))
                Url = "http://" + Url;
            try
            {
                Status = "Abrindo url...";
                var newManga = await MangaSite.GetMangaInfo(Url, ct);
                Status = $"Mostrando {newManga.Chapters.Count} capítulos";
                _openedMangas.Add(newManga);
                CurrentManga = newManga;
            }
            catch
            {
                Status = "Ocorreu um erro ao abrir a url";
            }
        }

        private IObservable<bool> CanOpenUrl()
        {
            return this.WhenAnyValue(vm => vm.Url,
                url => !string.IsNullOrWhiteSpace(url) && IsStringValidUrl(url));
        }

        private void SelectMangaInternal(string isPrevious)
        {
            if (_openedMangas.Count < 2) return;
            var previous = bool.Parse(isPrevious);
            var currentMangaPos = _openedMangas.FindIndex(manga => ReferenceEquals(manga, CurrentManga));
            var elementPos = previous ? currentMangaPos - 1 : currentMangaPos + 1;
            var condition = previous ? currentMangaPos > 0 : elementPos < _openedMangas.Count;
            if (!condition)
                elementPos = previous ? _openedMangas.Count - 1 : 0;
            CurrentManga = _openedMangas.ElementAt(elementPos);
            Status = $"Mostrando {CurrentManga.Chapters.Count} capítulos";
        }

        private void AddChaptersToDownloadInternal(IEnumerable<Chapter> chapters)
        {
            foreach (var chapter in chapters
                .ToList()
                .Where(chapter =>
                    ChaptersToDownload.All(chapterToDownload => chapterToDownload.Url != chapter.Url))
                .Reverse())
                ChaptersToDownload.Add(new ChapterToDownload(CurrentManga, chapter));
        }

        private IObservable<bool> CanAddChaptersToDownload()
        {
            return this.WhenAnyValue(x => x.CurrentManga).Select(x => x != null);
        }

        private void RmChaptersFromDownloadInternal(IList chapters)
        {
            if (chapters.Count < 1) return;
            foreach (var chapter in chapters.Cast<ChapterToDownload>().ToList()) ChaptersToDownload.Remove(chapter);
        }

        private void OpenChapterFolderInternal(IList chapters)
        {
            if (chapters.Count < 1) return;
            var selectedItem = (ChapterToDownload) chapters[0];
            if (selectedItem.Folder != null) Process.Start(selectedItem.Folder);
        }

        private async Task BeginDownloadInternal(CancellationToken ct)
        {
            Progress = 0;
            _currentDownloads = new SemaphoreSlim(_config.ParallelDownloads);
            SetDownloadsPending();
            var checkForNewDownloadsTask = CheckForNewDownloads(ct);
            var getChaptersImageListTask = GetChaptersImageList(ct);
            await Task.Run(() => Task.WhenAll(checkForNewDownloadsTask, getChaptersImageListTask), ct);
        }

        private IObservable<bool> CanBeginDownload()
        {
            return this.WhenAnyValue(
                x => x._config.SavePath).Select(Directory.Exists);
        }

        private void OpenConfigWindowInternal()
        {
            new SettingsWindow().ShowDialog();
        }

        private async Task GetChaptersImageList(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var chapterToDownload in ChaptersToDownload.Where(x =>
                    x.Status == ChapterToDownload.ChapterStatus.Pending))
                    try
                    {
                        chapterToDownload.Status = ChapterToDownload.ChapterStatus.GettingImageList;
                        if (chapterToDownload.ImagesUrls.Count < 1) await chapterToDownload.GetImages(ct);
                        chapterToDownload.Status = ChapterToDownload.ChapterStatus.ReadyToDownload;
                    }
                    catch
                    {
                        chapterToDownload.Status = ChapterToDownload.ChapterStatus.Error;
                    }

                await Task.Delay(1000, ct);
            }
        }

        private async Task CheckForNewDownloads(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                Progress = ChaptersToDownload.Sum(ch => ch.Progress) / ChaptersToDownload.Count;
                if (NoNewDownloads) await CancelDownload.Execute();

                if (AnyNewDownload)
                {
                    await _currentDownloads.WaitAsync(ct);
                    _ = DownloadChapterImages(ChapterReadyToDownload, ct);
                }

                await Task.Delay(500, ct);
            }
        }

        private async Task DownloadChapterImages(ChapterToDownload chapterToDownload, CancellationToken ct)
        {
            var gotAnyDownloadError = false;
            chapterToDownload.Status = ChapterToDownload.ChapterStatus.Downloading;
            var mangaDirectoryPath =
                Path.Combine(_config.SavePath, Utils.RemoveInvalidChars(chapterToDownload.Manga.Name));
            chapterToDownload.Folder = Path.Combine(mangaDirectoryPath, $"Capítulo {chapterToDownload.Name}");
            Directory.CreateDirectory(chapterToDownload.Folder);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
            if (!chapterToDownload.Chapter.Url.Contains("manganelos.com"))
                httpClient.DefaultRequestHeaders.Add("Referer", chapterToDownload.Chapter.Url);
            httpClient.Timeout = TimeSpan.FromSeconds(60);
            try
            {
                await httpClient.SaveImageFromUrl(chapterToDownload.Manga.Cover,
                    Path.Combine(mangaDirectoryPath, "cover.jpg"), ct);
            }
            catch
            {
                // ignored
            }

            foreach (var (imageUrl, index) in chapterToDownload.ImagesUrls.WithIndex())
            {
                var imageNum = index + 1;
                var imageName = _config.AddLeadingZerosImage
                    ? Utils.AddLeadingZeroes(imageNum, chapterToDownload.ImagesUrls.Count)
                    : imageNum.ToString();
                var imageFullPath = Path.Combine(chapterToDownload.Folder, $"{imageName}.jpg");
                if (!File.Exists(imageFullPath))
                    try
                    {
                        await httpClient.SaveImageFromUrl(imageUrl, imageFullPath, ct);
                    }
                    catch
                    {
                        gotAnyDownloadError = true;
                        continue;
                    }

                chapterToDownload.SetDownloadProgressStatus(imageNum);
            }

            chapterToDownload.Status = gotAnyDownloadError
                ? ChapterToDownload.ChapterStatus.Incomplete
                : ChapterToDownload.ChapterStatus.Complete;
            _currentDownloads.Release();
        }

        private void SetDownloadsPending()
        {
            foreach (var chapter in ChaptersToDownload.Where(
                ch => ch.Status != ChapterToDownload.ChapterStatus.Complete))
                chapter.Status = ChapterToDownload.ChapterStatus.Pending;
        }

        private bool IsStringValidUrl(string urlString)
        {
            try
            {
                _ = new Uri(urlString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}