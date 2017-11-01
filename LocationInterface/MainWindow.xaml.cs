using System.Windows;
using System.Windows.Controls;
using LocationInterface.Pages;
using LocationInterface.Utils;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected Common Common { get; set; }

        protected HomePage HomePage { get; set; }
        protected DataViewerPage DataViewerPage { get; set; }
        protected SettingsPage SettingsPage { get; set; }
        protected MapViewPage MapViewPage { get; set; }
        protected RawDataPage RawDataPage { get; set; }
        protected AnalysisPage AnalysisPage { get; set; }

        /// <summary>
        /// Initialize the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create a new instance of the common properties and methods class
            Common = new Common();

            // Load all the application's pages
            HomePage = new HomePage(ShowDataViewerPage, ShowMapPage, ShowRawDataPage, ShowSettingsPage, ShowAnalysisPage);
            SettingsPage = new SettingsPage(ShowPreviousPage);
            MapViewPage = new MapViewPage(Common, ShowHomePage);
            RawDataPage = new RawDataPage(Common, ShowPreviousPage);
            AnalysisPage = new AnalysisPage(Common, ShowPreviousPage);
            DataViewerPage = new DataViewerPage(Common, ShowPreviousPage);

            // Set the window to launch in the center of the screen
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            // Show the hompage (as the first page when the application opens)
            ShowHomePage();
        }

        /// <summary>
        /// Callback to Show the home page
        /// </summary>
        protected void ShowHomePage()
        {
            ShowPage(HomePage);
        }
        /// <summary>
        /// Callback to Show the settings page
        /// </summary>
        protected void ShowSettingsPage()
        {
            SettingsPage.LoadSettings();
            ShowPage(SettingsPage);
        }
        /// <summary>
        /// Callback to show the data viewer page
        /// </summary>
        protected void ShowDataViewerPage()
        {
            DataViewerPage.UpdateTable();
            ShowPage(DataViewerPage);
        }
        /// <summary>
        /// Callback to show the previous page
        /// </summary>
        protected void ShowPreviousPage()
        {
            ShowPage(Common.PreviousPage);
        }
        /// <summary>
        /// Callback to show the map page
        /// </summary>
        protected void ShowMapPage()
        {
            MapViewPage.StartPolling();
            ShowPage(MapViewPage);
        }
        /// <summary>
        /// Callback to show the raw data page
        /// </summary>
        protected void ShowRawDataPage()
        {
            RawDataPage.LoadTables();
            ShowPage(RawDataPage);
        }
        /// <summary>
        /// Callback to show the analysis page
        /// </summary>
        protected void ShowAnalysisPage()
        {
            ShowPage(AnalysisPage);
        }
        /// <summary>
        /// Show a given page
        /// </summary>
        /// <param name="page">The page to display on the main window</param>
        protected void ShowPage(Page page)
        {
            // Log the current page as previous page
            Common.PreviousPage = Common.CurrentPage;
            // Switch the content of the inner frame to the current page
            DataContext = frame.Content = Common.CurrentPage = page;
        }
    }
}
