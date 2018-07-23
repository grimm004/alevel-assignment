using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

namespace LocationInterface.Utils
{
    public class Common
    {
        public Database LocationDatabase { get; }
        public Table[] LoadedDataTables { get; protected set; }

        /// <summary>
        /// Initialise the common class
        /// </summary>
        public Common()
        {
            // Initialise a new binary database
            LocationDatabase = new BINDatabase(SettingsManager.Active.LocationDataFolder);
            // Initialise a new table array for selected tables with no items
            LoadedDataTables = new Table[0];
        }

        /// <summary>
        /// Clean up anytrhing on close
        /// </summary>
        public void OnClose()
        { }

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
    }
}
