using LocationInterface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for DataViewerPage.xaml
    /// </summary>
    public partial class DataViewerPage : Page
    {
        protected Action ShowPreviousPage { get; set; }
        protected Action<LocationDataFile[]> LoadTables { get; set; }
        protected List<LocationDataFile> SelectedDataFiles { get; set; }

        public DataViewerPage(Action ShowPreviousPage)
        {
            this.ShowPreviousPage = ShowPreviousPage;
            InitializeComponent();
            LoadTable();
        }

        public void UpdateTable()
        {
            App.VerifyFiles();
            LoadTable();
        }
        protected void LoadTable()
        {
            dataFiles.Items.Clear();
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) dataFiles.Items.Add(currentFile);
        }
        protected void SubmitSelection()
        {
            LoadTables?.Invoke(SelectedDataFiles.ToArray());
        }

        private void DataFilesSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            SelectedDataFiles = dataFiles.SelectedItems.Cast<LocationDataFile>().ToList();
        }
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(SelectedDataFiles.Count);
            UpdateTable();
        }
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            SubmitSelection();
        }
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }
    }
}
