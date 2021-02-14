using ReactiveUI;
using System;

namespace KMD.ViewModels
{
    public class Config : ReactiveObject
    {
        private static readonly Lazy<Config> config = new Lazy<Config>(() => new Config());
        public static Config Instance => config.Value;

        private string _savePath = Properties.Settings.Default.savePath;
        public string SavePath
        {
            get => _savePath;
            set
            {
                Properties.Settings.Default.savePath = value;
                Properties.Settings.Default.Save();
                this.RaiseAndSetIfChanged(ref _savePath, value);
            }
        }

        private int _parallelDownloads = Properties.Settings.Default.parallelDownloads;
        public int ParallelDownloads
        {
            get => _parallelDownloads;
            set
            {
                Properties.Settings.Default.parallelDownloads = value;
                Properties.Settings.Default.Save();
                this.RaiseAndSetIfChanged(ref _parallelDownloads, value);
            }
        }

        private bool _addLeadingZerosImage = Properties.Settings.Default.addLeadingZerosImage;
        public bool AddLeadingZerosImage
        {
            get => _addLeadingZerosImage;
            set
            {
                Properties.Settings.Default.addLeadingZerosImage = value;
                Properties.Settings.Default.Save();
                this.RaiseAndSetIfChanged(ref _addLeadingZerosImage, value);
            }
        }
        
        private bool _nameChaptersByIndex = Properties.Settings.Default.addLeadingZerosImage;
        public bool NameChaptersByIndex
        {
            get => _nameChaptersByIndex;
            set
            {
                Properties.Settings.Default.addLeadingZerosImage = value;
                Properties.Settings.Default.Save();
                this.RaiseAndSetIfChanged(ref _nameChaptersByIndex, value);
            }
        }

        private Config()
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
        }
    }
}
