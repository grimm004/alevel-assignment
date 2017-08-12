using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LocationInterface.Pages;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public HomePage HomePage { get; protected set; }
        public DataViewerPage DataViewerPage { get; protected set; }
        public SettingsPage SettingsPage { get; protected set; }
        public MapViewPage MapPage { get; protected set; }
        protected Page PreviousPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            HomePage = new HomePage(ShowDataViewerPage, ShowMapPage);
            SettingsPage = new SettingsPage(ShowPreviousPage);
            MapPage = new MapViewPage(ShowHomePage, HomePage.Database);
            DataViewerPage = new DataViewerPage(ShowPreviousPage, MapPage.LoadTables);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.KeyDown += KeyPress;

            ShowHomePage();
        }

        protected Key[] keys = new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right };
        protected void KeyPress(object sender, KeyEventArgs e)
        {
            foreach (Key key in keys)
                if (e.Key == key)
                {
                    Keyboard.Focus(MapPage.canvas);
                    MapPage.canvas.Focus();
                    e.Handled = true;
                    break;
                }
        }

        protected void ShowHomePage()
        {
            ShowPage(HomePage);
        }
        protected void ShowSettingsPage()
        {
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
            MapPage.LoadRecords();
            ShowPage(MapPage);
        }
        protected void ShowPage(Page page)
        {
            PreviousPage = (Page)frame.Content;
            frame.Content = page;
        }
    }
}
