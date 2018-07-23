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
            FileManagerPageFrame.Content = FileManagerPage = new FileManagerPage(Common);
            SettingsPageFrame.Content = SettingsPage = new SettingsPage(Common);
            MapViewerPageFrame.Content = MapViewPage = new MapViewPage(Common);
            RawDataPageFrame.Content = RawDataPage = new RawDataPage(Common);
            AnalysisPageFrame.Content = AnalysisPage = new AnalysisPage(Common);

            //DataContext = this;
        }
        
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
            if (PageSelector.SelectedContent as Frame != null && e.Source is TabControl)
            {
                Page page = (PageSelector.SelectedContent as Frame).Content as Page;
                switch (page)
                {
                    case SettingsPage settingsPage:
                        settingsPage.LoadSettings();
                        break;
                    case FileManagerPage fileManagerPage:
                        fileManagerPage.UpdateTable();
                        break;
                    default:
                        break;
                }

                DataContext = page;
            }
            e.Handled = true;
        }
    }
}
