using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using LocationInterface.Utils;
using System.Threading;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for RawDataPage.xaml
    /// </summary>
    public partial class RawDataPage : Page
    {
        protected Action ShowPreviousPage { get; }
        protected Database Database { get; set; }
        protected Table[] LoadedTables { get; set; }

        public RawDataPage(Action ShowPreviousPage)
        {
            this.ShowPreviousPage = ShowPreviousPage;
            Database = new BINDatabase("LocationData");
            LoadedTables = new Table[0];
            InitializeComponent();
        }

        public void SetTables(LocationDataFile[] dataFiles)
        {
            Database = new BINDatabase("LocationData");
            LoadedTables = new Table[dataFiles.Length];
            for (int i = 0; i < dataFiles.Length; i++)
                LoadedTables[i] = Database.GetTable(dataFiles[i].TableName);
        }

        public void LoadTables()
        {
            new Thread(() =>
            {
                rawData.Dispatcher.Invoke(rawData.Items.Clear);
                List<LocationRecord> locationRecordBuffer = new List<LocationRecord>();
                foreach (Table table in LoadedTables)
                    foreach (Record currentLocationRecord in table.GetRecords())
                    {
                        locationRecordBuffer.Add(currentLocationRecord.ToObject<LocationRecord>());
                        if (locationRecordBuffer.Count == SettingsManager.Active.RawDataRecordBuffer) PopulateDataGrid(ref locationRecordBuffer);
                    }
                PopulateDataGrid(ref locationRecordBuffer);
            })
            { IsBackground = true }.Start();
        }

        public void PopulateDataGrid(ref List<LocationRecord> nextBuffer)
        {
            List<LocationRecord> currentBuffer = nextBuffer;
            rawData.Dispatcher.Invoke(() =>
            {
                foreach (LocationRecord locationRecord in currentBuffer)
                    rawData.Items.Add(locationRecord);
            });
            nextBuffer.Clear();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }
    }
}
