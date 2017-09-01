using LocationInterface.Utils;
using System.Windows;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for SelectionInformationWindow.xaml
    /// </summary>
    public partial class SelectionInformationWindow : Window
    {
        protected LocationDataFile[] SelectedDataFiles { get; set; }
        protected int TotalRecords
        {
            get
            {
                int recordCount = 0;
                foreach (LocationDataFile dataFile in SelectedDataFiles) recordCount += (int)dataFile.RecordCount;
                return recordCount;
            }
        }
        protected int AverageRecords
        {
            get
            {
                return TotalRecords / SelectedDataFiles.Length;
            }
        }

        public SelectionInformationWindow(LocationDataFile[] selectedDataFiles)
        {
            SelectedDataFiles = selectedDataFiles;
            InitializeComponent();

            selectedTablesLabel.Content = selectedDataFiles.Length.ToString();
            totalRecordsLabel.Content = selectedDataFiles.Length > 0 ? TotalRecords.ToString() : "";
            meanRecordsLabel.Content = selectedDataFiles.Length > 0 ? AverageRecords.ToString() : "";
        }
    }
}
