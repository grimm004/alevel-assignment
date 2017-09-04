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
        private Action ShowAnalysisPage { get; set; }

        /// <summary>
        /// Initialise the home page
        /// </summary>
        /// <param name="ShowDataViewerPage">Callback to show the data viewer page</param>
        /// <param name="ShowMapPage">Callback to show the map page</param>
        /// <param name="ShowRawDataPage">Callback to show the raw data page</param>
        /// <param name="ShowSettingsPage">Callback to show the settings page</param>
        /// <param name="ShowAnalysisPage">Callback to show the analysis page</param>
        public HomePage(Action ShowDataViewerPage, Action ShowMapPage, Action ShowRawDataPage, Action ShowSettingsPage, Action ShowAnalysisPage)
        {
            InitializeComponent();

            this.ShowDataViewerPage = ShowDataViewerPage;
            this.ShowMapPage = ShowMapPage;
            this.ShowRawDataPage = ShowRawDataPage;
            this.ShowSettingsPage = ShowSettingsPage;
            this.ShowAnalysisPage = ShowAnalysisPage;
        }

        // The below events just show their corresponding pages

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
        private void AnalysisPageButtonClick(object sender, RoutedEventArgs e)
        {
            ShowAnalysisPage?.Invoke();
        }

        // Exit the program
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
