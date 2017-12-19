using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;
using LocationInterface.Pages;
using System;
using System.Windows.Controls;

namespace LocationInterface.Utils
{
    public class Common
    {
        public Database LocationDatabase { get; }
        public Table[] LoadedDataTables { get; protected set; }
        public Page CurrentPage { get; set; }
        public Page PreviousPage { get; set; }
        protected Action<Page> ShowPageCallback { get; }

        protected HomePage HomePage { get; }
        protected DataViewerPage DataViewerPage { get; }
        protected SettingsPage SettingsPage { get; }
        protected MapViewPage MapViewPage { get; }
        protected RawDataPage RawDataPage { get; }
        protected AnalysisPage AnalysisPage { get; }

        /// <summary>
        /// Initialise the common class
        /// </summary>
        public Common(Action<Page> ShowPageCallback)
        {
            this.ShowPageCallback = ShowPageCallback;

            // Initialise a new binary database
            LocationDatabase = new BINDatabase(SettingsManager.Active.LocationDataFolder);
            // Initialise a new table array for selected tables with no items
            LoadedDataTables = new Table[0];

            // Load all the application's pages
            HomePage = new HomePage(this);
            SettingsPage = new SettingsPage(this);
            MapViewPage = new MapViewPage(this);
            RawDataPage = new RawDataPage(this);
            AnalysisPage = new AnalysisPage(this);
            DataViewerPage = new DataViewerPage(this);
        }

        /// <summary>
        /// Load the selected tables from a LocationDataFile array
        /// </summary>
        /// <param name="dataFiles">The datafiles to load</param>
        public void LoadTables(LocationDataFile[] dataFiles)
        {
            // Initialise the array of selected data tables
            LoadedDataTables = new Table[dataFiles.Length];
            // Loop through the datafiles and load each data file to the table
            for (int i = 0; i < dataFiles.Length; i++)
                LoadedDataTables[i] = LocationDatabase.GetTable(dataFiles[i].TableName);
        }

        /// <summary>
        /// Callback to Show the home page
        /// </summary>
        public void ShowHomePage()
        {
            ShowPageCallback(HomePage);
        }
        /// <summary>
        /// Callback to Show the settings page
        /// </summary>
        public void ShowSettingsPage()
        {
            SettingsPage.LoadSettings();
            ShowPageCallback(SettingsPage);
        }
        /// <summary>
        /// Callback to show the data viewer page
        /// </summary>
        public void ShowDataViewerPage()
        {
            DataViewerPage.UpdateTable();
            ShowPageCallback(DataViewerPage);
        }
        /// <summary>
        /// Callback to show the previous page
        /// </summary>
        public void ShowPreviousPage()
        {
            ShowPageCallback(PreviousPage);
        }
        /// <summary>
        /// Callback to show the map page
        /// </summary>
        public void ShowMapPage()
        {
            ShowPageCallback(MapViewPage);
        }
        /// <summary>
        /// Callback to show the raw data page
        /// </summary>
        public void ShowRawDataPage()
        {
            RawDataPage.LoadTables();
            ShowPageCallback(RawDataPage);
        }
        /// <summary>
        /// Callback to show the analysis page
        /// </summary>
        public void ShowAnalysisPage()
        {
            ShowPageCallback(AnalysisPage);
        }
    }
}
