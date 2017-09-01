using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LocationInterface.Pages;
using LocationInterface.Utils;
using DatabaseManagerLibrary.BIN;
using DatabaseManagerLibrary;

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
        protected Page PreviousPage { get; set; }
        protected Page CurrentPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Common = new Common();

            HomePage = new HomePage(ShowDataViewerPage, ShowMapPage, ShowRawDataPage, ShowSettingsPage, ShowAnalysisPage);
            SettingsPage = new SettingsPage(ShowPreviousPage, UpdateSettings);
            MapViewPage = new MapViewPage(Common, ShowHomePage);
            RawDataPage = new RawDataPage(Common, ShowPreviousPage);
            AnalysisPage = new AnalysisPage(Common, ShowPreviousPage);
            DataViewerPage = new DataViewerPage(Common, ShowPreviousPage);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            KeyDown += KeyPress;

            ShowHomePage();
        }

        protected Key[] keys = new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right };
        protected void KeyPress(object sender, KeyEventArgs e)
        {
            if (CurrentPage.GetType() == typeof(MapViewPage))
                foreach (Key key in keys)
                    if (e.Key == key)
                    {
                        Keyboard.Focus(MapViewPage.canvas);
                        MapViewPage.canvas.Focus();
                        e.Handled = true;
                        break;
                    }
        }

        protected void UpdateSettings()
        {
            SettingsManager.Save();
        }

        protected void ShowHomePage()
        {
            ShowPage(HomePage);
        }
        protected void ShowSettingsPage()
        {
            SettingsPage.LoadSettings();
            ShowPage(SettingsPage);
        }
        protected void ShowDataViewerPage()
        {
            DataViewerPage.UpdateTable();
            ShowPage(DataViewerPage);
        }
        protected void ShowPreviousPage()
        {
            ShowPage(PreviousPage);
        }
        protected void ShowMapPage()
        {
            MapViewPage.LoadTables();
            ShowPage(MapViewPage);
        }
        protected void ShowRawDataPage()
        {
            RawDataPage.LoadTables();
            ShowPage(RawDataPage);
        }
        protected void ShowAnalysisPage()
        {
            ShowPage(AnalysisPage);
        }
        protected void ShowPage(Page page)
        {
            PreviousPage = CurrentPage;
            frame.Content = CurrentPage = page;
        }
    }
}
