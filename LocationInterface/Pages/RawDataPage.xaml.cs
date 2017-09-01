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
        protected Common Common { get; }

        public RawDataPage(Common common, Action ShowPreviousPage)
        {
            Common = common;
            this.ShowPreviousPage = ShowPreviousPage;
            InitializeComponent();
        }

        public void LoadTables()
        {
            new Thread(() =>
            {
                rawData.Dispatcher.Invoke(rawData.Items.Clear);
                List<LocationRecord> locationRecordBuffer = new List<LocationRecord>();
                foreach (Table table in Common.LoadedDataTables)
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
