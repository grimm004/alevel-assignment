using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LocationInterface.Pages;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected static HomePage homePage;
        protected static DataViewerPage dataViewerPage;
        protected static SettingsPage settingsPage;
        protected static MapViewPage mapPage;

        protected Page previousPage;

        public MainWindow()
        {
            InitializeComponent();

            homePage = new HomePage(ShowDataViewerPage, ShowMapPage);
            dataViewerPage = new DataViewerPage(ShowPreviousPage);
            settingsPage = new SettingsPage(ShowPreviousPage);
            mapPage = new MapViewPage(ShowHomePage);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.KeyDown += KeyPress;

            //ShowHomePage();
            ShowMapPage();
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
                Keyboard.Focus(mapPage.canvas);
                mapPage.canvas.Focus();
            }
        }

        public void ShowHomePage()
        {
            ShowPage(homePage);
        }

        public void ShowSettingsPage()
        {
            ShowPage(settingsPage);
        }

        public void ShowDataViewerPage()
        {
            dataViewerPage.UpdateTable();
            ShowPage(dataViewerPage);
        }

        public void ShowPreviousPage()
        {
            ShowPage(previousPage);
        }

        public void ShowMapPage()
        {
            ShowPage(mapPage);
        }

        protected void ShowPage(Page page)
        {
            previousPage = (Page)frame.Content;
            frame.Content = page;
        }
    }
}
