using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Windows;
using DatabaseManager;
using System.IO;
using Newtonsoft.Json;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CSVDatabase database;
        public static DataIndex dataIndex;
        
        public App()
        {
            //database = new CSVDatabase("Ship");
            if (!Directory.Exists(@"LocationData")) Directory.CreateDirectory(@"LocationData");
            if (!File.Exists(@"LocationData\index.json") || (dataIndex = DataIndex.LoadIndex()) == null) { File.Create(@"LocationData\index.json").Close(); dataIndex = new DataIndex(); }
            else VerifyFiles();
            dataIndex.SaveIndex();
        }

        public static void VerifyFiles()
        {
            for (int i = dataIndex.LocationDataFiles.Count - 1; i >= 0; i--) if (!dataIndex.LocationDataFiles[i].Exists) dataIndex.LocationDataFiles.RemoveAt(i);
            dataIndex.SaveIndex();
        }
    }

    public class DataIndex
    {
        public List<LocationDataFile> LocationDataFiles { get; set; }

        public DataIndex()
        {
            LocationDataFiles = new List<LocationDataFile>();
        }

        public static DataIndex LoadIndex()
        {
            byte[] data = File.ReadAllBytes(@"LocationData\index.json");
            return JsonConvert.DeserializeObject<DataIndex>(Encoding.UTF8.GetString(data));
        }

        public void SaveIndex()
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
            File.WriteAllBytes(@"LocationData\index.json", data);
        }
    }

    public class LocationDataFile
    {
        public string LocationIdentifier { get; set; }
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }
        public bool Exists { get { return File.Exists(@"LocationData\" + FileName); } }

        public override string ToString()
        {
            return string.Format("LocationFile('{0}', {1}, {2})", LocationIdentifier, DateTime, FileName);
        }
    }
}
