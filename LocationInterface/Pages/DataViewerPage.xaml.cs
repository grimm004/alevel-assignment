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

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for DataViewerPage.xaml
    /// </summary>
    public partial class DataViewerPage : Page
    {
        private Action ShowPreviousPage;

        public DataViewerPage(Action ShowPreviousPage)
        {
            this.ShowPreviousPage = ShowPreviousPage;
            InitializeComponent();
            LoadTable();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }

        public void UpdateTable()
        {
            App.VerifyFiles();
            LoadTable();
        }

        private void LoadTable()
        {
            dataFiles.Items.Clear();
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) dataFiles.Items.Add(currentFile);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(selected.Count);
            UpdateTable();
        }
        
        List<LocationDataFile> selected;
        private void DataFiles_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            selected = dataFiles.SelectedItems.Cast<LocationDataFile>().ToList();
        }
    }
}
