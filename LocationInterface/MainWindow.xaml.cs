using System.ComponentModel;
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

        protected HomePage HomePage { get; }
        protected FileManagerPage FileManagerPage { get; }
        protected SettingsPage SettingsPage { get; }
        protected MapViewPage MapViewPage { get; }
        protected RawDataPage RawDataPage { get; }
        protected AnalysisPage AnalysisPage { get; }

        /// <summary>
        /// Initialize the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create a new instance of the common properties and methods class
            Common = new Common();

            // Set the window to launch in the center of the screen
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Load all the application's pages
            //HomePage = new HomePage(Common);
            
            SettingsPage = new SettingsPage(Common);
            SettingsPageFrame.Content = SettingsPage;
            MapViewPage = new MapViewPage(Common);
            MapViewerPageFrame.Content = MapViewPage;
            RawDataPage = new RawDataPage(Common);
            RawDataPageFrame.Content = RawDataPage;
            AnalysisPage = new AnalysisPage(Common);
            AnalysisPageFrame.Content = AnalysisPage;
            FileManagerPage = new FileManagerPage(Common);
            FileManagerPageFrame.Content = FileManagerPage;

            //// Show the hompage (as the first page when the application opens)
            //Common.ShowHomePage();
        }

        ///// <summary>
        ///// Show a given page
        ///// </summary>
        ///// <param name="page">The page to display on the main window</param>
        //protected void ShowPage(Page page)
        //{
        //    // Log the current page as previous page
        //    Common.PreviousPage = Common.CurrentPage;
        //    // Switch the content of the inner frame to the current page
        //    DataContext = frame.Content = Common.CurrentPage = page;
        //}

        /// <summary>
        /// Action when the window is closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            Common.OnClose();
            base.OnClosing(e);
        }

        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
