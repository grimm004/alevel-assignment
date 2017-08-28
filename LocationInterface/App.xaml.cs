using System.Windows;
using System.IO;
using LocationInterface.Utils;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DataIndex DataIndex { get; protected set; }
        
        public App()
        {
            if (!Directory.Exists(@"DataCache")) Directory.CreateDirectory(@"DataCache");
            if (!Directory.Exists(@"LocationData")) Directory.CreateDirectory(@"LocationData");
            if (!File.Exists(@"LocationData\index.json") || (DataIndex = DataIndex.LoadIndex()) == null) { File.Create(@"LocationData\index.json").Close(); DataIndex = new DataIndex(); }
            else VerifyFiles();

            Database database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention : ".csv");
            if (database.GetTable("Contacts") == null) database.CreateTable("Contacts", new CSVTableFields("Name:string,EmailAddress:string"));
            if (database.GetTable("Presets") == null) database.CreateTable("Presets", new CSVTableFields("Name:string,Subject:string,Body:string"));
            database.SaveChanges();

            DataIndex.SaveIndex();
        }

        public static void VerifyFiles()
        {
            for (int i = DataIndex.LocationDataFiles.Count - 1; i >= 0; i--) if (!DataIndex.LocationDataFiles[i].Exists) DataIndex.LocationDataFiles.RemoveAt(i);
            DataIndex.SaveIndex();
        }
    }
}
