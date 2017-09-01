using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

namespace LocationInterface.Utils
{
    public class Common
    {
        public Database LocationDatabase { get; }
        public Table[] LoadedDataTables { get; protected set; }

        public Common()
        {
            LocationDatabase = new BINDatabase(SettingsManager.Active.LocationDataFolder);
            LoadedDataTables = new Table[0];
        }

        public void LoadTables(LocationDataFile[] dataFiles)
        {
            LoadedDataTables = new Table[dataFiles.Length];
            for (int i = 0; i < dataFiles.Length; i++)
                LoadedDataTables[i] = LocationDatabase.GetTable(dataFiles[i].TableName);
        }
    }
}
