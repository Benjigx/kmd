using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;
using System;
using System.Reactive;

namespace KMD.ViewModels
{
    public class SettingsWindowVM : ReactiveObject
    {
        public Config config = Config.Instance;

        public SettingsWindowVM()
        {
            SelectPath = ReactiveCommand.Create(SelectPathInternal);
            SaveSettings = ReactiveCommand.Create(SaveSettingsInternal);
        }

        private void SelectPathInternal()
        {
            var folder = ShowSaveDialog();
            if (folder != null)
                config.SavePath = folder;
        }

        private void SaveSettingsInternal()
        {
            Properties.Settings.Default.Save();
        }

        private string ShowSaveDialog()
        {
            var saveDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Environment.SpecialFolder.Desktop.ToString(),
                Title = "Selecione a pasta de destino."
            };
            return saveDialog.ShowDialog() == CommonFileDialogResult.Ok ? saveDialog.FileName : null;
        }

        public ReactiveCommand<Unit, Unit> SelectPath { get; }
        public ReactiveCommand<Unit, Unit> SaveSettings { get; }
    }
}
