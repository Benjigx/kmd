using DynamicData;
using KMD.ViewModels;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace KMD.Views
{
    /// <summary>
    /// Interaction logic for MangaDownloadView.xaml
    /// </summary>
    public partial class MangaDownloadView : IViewFor<MangaDownloadVM>
    {
        public MangaDownloadView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel,
                    vm => vm.Url,
                    v => v.UrlTextBox.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.OpenUrl,
                    v => v.OpenUrlButton)
                    .DisposeWith(d);

                UrlTextBox.Events().KeyDown
                    .Where(x => x.Key == Key.Return || x.Key == Key.Enter)
                    .Select(x => Unit.Default)
                    .InvokeCommand(ViewModel, x => x.OpenUrl)
                    .DisposeWith(d);

                SearchChapterTextBox.TextChanged += SearchChapter;

                UrlTextBox.PreviewMouseLeftButtonDown += UrlInput_PreviewMouseLeftButton;

                UrlTextBox.GotKeyboardFocus += UrlInput_OnKeyboardFocus;

                this.BindCommand(ViewModel,
                    vm => vm.SelectManga,
                    v => v.SelectMangaPrevious)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.SelectManga,
                    v => v.SelectMangaForward)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.CurrentManga.Name,
                    v => v.MangaTitleTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.CurrentManga.Cover,
                    v => v.MangaCoverImage.Source)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.CurrentManga.Chapters,
                    v => v.MangaChaptersListView.ItemsSource)
                    .DisposeWith(d);

                MangaChaptersListView.Events().KeyDown
                    .Where(x => x.Key == Key.Return || x.Key == Key.Enter)
                    .Select(x => MangaChaptersListView.SelectedItems.Cast<Chapter>())
                    .InvokeCommand(ViewModel, x => x.AddChaptersToDownload)
                    .DisposeWith(d);

                MangaChaptersListView.Events().MouseDoubleClick
                    .Where(x => x.LeftButton == MouseButtonState.Pressed)
                    .Select(x => MangaChaptersListView.SelectedItems.Cast<Chapter>())
                    .InvokeCommand(ViewModel, x => x.AddChaptersToDownload)
                    .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.Status,
                    v => v.StatusTextBlock.Text)
                    .DisposeWith(d);

                DownloadSelectedMenuItem.Events().Click
                    .Select(x => MangaChaptersListView.SelectedItems.Cast<Chapter>())
                    .InvokeCommand(ViewModel, x => x.AddChaptersToDownload)
                    .DisposeWith(d);

                DownloadAllMenuItem.Events().Click
                    .Select(x => MangaChaptersListView.ItemsSource)
                    .InvokeCommand(ViewModel, x => x.AddChaptersToDownload)
                    .DisposeWith(d);

                ViewModel.WhenAnyValue(x => x.ChaptersToDownload)
                    .Select(x => x)
                    .BindTo(this, x => x.ChaptersToDownloadListView.ItemsSource)
                    .DisposeWith(d);

                ChaptersToDownloadListView.Events().KeyDown
                    .Where(x => x.Key == Key.Delete)
                    .Select(x => ChaptersToDownloadListView.SelectedItems)
                    .InvokeCommand(ViewModel, x => x.RmChaptersFromDownload)
                    .DisposeWith(d);

                ChaptersToDownloadListView.Events().MouseDoubleClick
                    .Where(x => x.LeftButton == MouseButtonState.Pressed)
                    .Select(x => ChaptersToDownloadListView.SelectedItems)
                    .InvokeCommand(ViewModel, x => x.OpenChapterFolder)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.BeginDownload,
                    v => v.DownloadButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.CancelDownload,
                    v => v.CancelButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.OpenConfigWindow,
                    v => v.SettingsButton)
                    .DisposeWith(d);
            });
        }

        private void UrlInput_PreviewMouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            if (UrlTextBox.IsKeyboardFocusWithin) return;
            e.Handled = true;
            UrlTextBox.Focus();
        }

        private void UrlInput_OnKeyboardFocus(object sender, RoutedEventArgs e)
        {
            UrlTextBox.SelectAll();
        }

        private void SearchChapter(object sender, TextChangedEventArgs e)
        {
            var currentManga = ViewModel.CurrentManga;
            if (currentManga == null) return;
            var items = currentManga.Chapters.Where(chapter => chapter.Name.StartsWith(SearchChapterTextBox.Text));
            var index = currentManga.Chapters.IndexOf(items.LastOrDefault());
            if (index == -1) return;
            MangaChaptersListView.ScrollIntoView(MangaChaptersListView.Items[MangaChaptersListView.Items.Count - 1]);
            MangaChaptersListView.ScrollIntoView(MangaChaptersListView.Items[index]);
        }

        public MangaDownloadVM ViewModel
        {
            get => (MangaDownloadVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MangaDownloadVM), typeof(MangaDownloadView), new PropertyMetadata(null));
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MangaDownloadVM)value;
        }
    }
}
