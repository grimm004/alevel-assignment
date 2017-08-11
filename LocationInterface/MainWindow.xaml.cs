using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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

        protected Key[] keys = new Key[] { Key.A, Key.D, Key.W, Key.S, Key.F, Key.G, Key.H, Key.T, Key.R, Key.Y, Key.Up, Key.Down, Key.Left, Key.Right };
        private void KeyPress(object sender, KeyEventArgs e)
        {
            foreach (Key key in keys)
                if (e.Key == key)
                {
                    Keyboard.Focus(mapPage.canvas);
                    mapPage.canvas.Focus();
                    e.Handled = true;
                    break;
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
