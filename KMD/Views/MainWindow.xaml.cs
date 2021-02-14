using KMD.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace KMD.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppBootstrapper AppBootstrapper { get; protected set; }

        public MainWindow()
        {
            InitializeComponent();
            AppBootstrapper = new AppBootstrapper();
            DataContext = AppBootstrapper;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (TitleBar.IsMouseDirectlyOver)
                    DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
