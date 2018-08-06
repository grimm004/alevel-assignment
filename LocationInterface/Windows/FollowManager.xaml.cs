using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for FollowManager.xaml
    /// </summary>
    public partial class FollowManagerWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public bool FollowEnabled { get; private set; }
        public string SelectedAddress { get { return SelectedAddressTextbox.Text; } }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        public FollowManagerWindow()
        {
            InitializeComponent();
            SelectedAddressTextbox.Text = "28:e0:2c:32:04:40";
            FollowEnabled = false;
        }
        
        private void SelectedAddressChanged(object sender, TextChangedEventArgs e)
        {
            SelectedAddressTextbox.Foreground = new SolidColorBrush(ValidAddress() ? Colors.Green : Colors.Red);
            Verify();
        }

        private bool ValidAddress()
        {
            return Regex.IsMatch(SelectedAddressTextbox.Text, @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        }

        private void FollowCheckboxEnabled(object sender, RoutedEventArgs e)
        {
            Verify();
        }

        private void FollowCheckboxDisabled(object sender, RoutedEventArgs e)
        {
            Verify();
        }

        private void Verify()
        {
            FollowEnabled = EnableFollowCheckbox.IsChecked.HasValue && EnableFollowCheckbox.IsChecked.Value && ValidAddress();
        }
    }
}
