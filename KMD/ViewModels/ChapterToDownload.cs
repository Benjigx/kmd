using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KMD.ViewModels
{
    public class ChapterToDownload : ReactiveObject
    {
        public Chapter Chapter { get; }
        public Manga Manga { get; }
        public ICollection<string> ImagesUrls { get; private set; } = new List<string>();
        public string Url => Chapter.Url;
        public string Name => Chapter.Name;
        public string Folder { get; set; }

        [Reactive]
        public string Status { get; set; } = ChapterStatus.Pending;

        [Reactive]
        public float Progress { get; set; }

        public ChapterToDownload(Manga manga, Chapter chapter)
        {
            Manga = manga;
            Chapter = chapter;
        }

        public void SetDownloadProgressStatus(int imgDownloaded)
        {
            Status = $"Baixando {imgDownloaded}/{ImagesUrls.Count}";
            Progress = 1f / ImagesUrls.Count * imgDownloaded;
        }

        public async Task GetImages(CancellationToken ct)
        {
            ImagesUrls = await MangaSite.GetChapterImages(Chapter, ct);
        }

        public static class ChapterStatus
        {
            public const string Pending = "Pendente";
            public const string GettingImageList = "Obtendo imagens";
            public const string ReadyToDownload = "Pronto para baixar";
            public const string Downloading = "Baixando";
            public const string Error = "Erro";
            public const string Incomplete = "Incompleto";
            public const string Complete = "Completo";
        }
    }
}
