using LocationInterface.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for FileRenameWindow.xaml
    /// </summary>
    public partial class FileRenameWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        public string ImageName { get; set; }

        public FileRenameWindow()
        {
            InitializeComponent();
            DataContext = this;
            ImageName = "";
        }

        public FileRenameWindow(string imageName)
        {
            InitializeComponent();
            DataContext = this;
            ImageName = imageName;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ImageName))
                MessageBox.Show("Please enter an image name.", "Invalid Image Name", MessageBoxButton.OK, MessageBoxImage.Error);
            if (File.Exists($"{ SettingsManager.Active.ImageFolder }\\{ ImageName }.bmp"))
                MessageBox.Show("An image with this name already exists.", "Invalid Image Name", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                DialogResult = true;
                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
