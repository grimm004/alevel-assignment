using System;
using System.Windows;
using System.Windows.Controls;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Action ShowDataViewerPage { get; set; }
        private Action ShowMapPage { get; set; }
        private Action ShowRawDataPage { get; set; }
        private Action ShowSettingsPage { get; set; }

        public HomePage(Action ShowDataViewerPage, Action ShowMapPage, Action ShowRawDataPage, Action ShowSettingsPage)
        {
            this.ShowDataViewerPage = ShowDataViewerPage;
            this.ShowMapPage = ShowMapPage;
            this.ShowRawDataPage = ShowRawDataPage;
            this.ShowSettingsPage = ShowSettingsPage;
            InitializeComponent();
        }

        private void ViewImportedFilesButtonClick(object sender, RoutedEventArgs e)
        {
            ShowDataViewerPage?.Invoke();
        }
        private void ViewMapViewerButtonClick(object sender, RoutedEventArgs e)
        {
            ShowMapPage?.Invoke();
        }
        private void RawDataViewerButtonClick(object sender, RoutedEventArgs e)
        {
            ShowRawDataPage?.Invoke();
        }
        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            ShowSettingsPage?.Invoke();
        }
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
