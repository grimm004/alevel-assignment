using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LocationInterface.Pages;
using LocationInterface.Utils;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected HomePage HomePage { get; set; }
        protected DataViewerPage DataViewerPage { get; set; }
        protected SettingsPage SettingsPage { get; set; }
        protected MapViewPage MapViewPage { get; set; }
        protected RawDataPage RawDataPage { get; set; }
        protected Page PreviousPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            HomePage = new HomePage(ShowDataViewerPage, ShowMapPage, ShowRawDataPage, ShowSettingsPage);
            SettingsPage = new SettingsPage(ShowPreviousPage, UpdateSettings);
            MapViewPage = new MapViewPage(ShowHomePage);
            RawDataPage = new RawDataPage(ShowPreviousPage);
            DataViewerPage = new DataViewerPage(ShowPreviousPage, new System.Action<Utils.LocationDataFile[]>[] { MapViewPage.SetTables, RawDataPage.SetTables });

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            KeyDown += KeyPress;

            ShowHomePage();
        }

        protected Key[] keys = new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right };
        protected void KeyPress(object sender, KeyEventArgs e)
        {
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
        protected void ShowPage(Page page)
        {
            PreviousPage = (Page)frame.Content;
            frame.Content = page;
        }
    }
}
