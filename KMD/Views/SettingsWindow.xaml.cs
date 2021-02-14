using KMD.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;

namespace KMD.Views
{
    /// <summary>
    /// Lógica interna para SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ReactiveWindow<SettingsWindowVM>
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ViewModel = new SettingsWindowVM();
            Owner = Application.Current.MainWindow;
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel,
                    vm => vm.config.SavePath,
                    v => v.DownloadPathTextBox.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                    vm => vm.SelectPath,
                    v => v.SelectDirButton)
                    .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.config.ParallelDownloads,
                    v => v.ParallelDownloadsSpinner.Value)
                    .DisposeWith(d);

                this.Bind(ViewModel,
                    vm => vm.config.AddLeadingZerosImage,
                    v => v.AddLeadingZerosToggle.IsChecked)
                    .DisposeWith(d);
            });
        }
    }
}
