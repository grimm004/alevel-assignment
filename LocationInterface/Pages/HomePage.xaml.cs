using LocationInterface.Utils;
using System.Windows;
using System.Windows.Controls;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        protected Common Common { get; }

        /// <summary>
        /// Initialise the home page
        /// </summary>
        public HomePage(Common common)
        {
            Common = common;
            InitializeComponent();
        }

        // The below events show their corresponding pages
        private void ViewImportedFilesButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowDataViewerPage();
        }
        private void ViewMapViewerButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowMapPage();
        }
        private void RawDataViewerButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowRawDataPage();
        }
        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowSettingsPage();
        }
        private void AnalysisPageButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowAnalysisPage();
        }

        // Exit the program
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
