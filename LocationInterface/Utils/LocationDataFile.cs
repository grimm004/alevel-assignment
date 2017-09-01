using System;
using System.IO;
using Newtonsoft.Json;
using DatabaseManagerLibrary.BIN;

namespace LocationInterface.Utils
{
    public class LocationDataFile
    {
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }

        [JsonIgnore]
        public string TableName { get { return Path.GetFileNameWithoutExtension(FileName); } }
        [JsonIgnore]
        public bool Exists { get { return File.Exists($"{ SettingsManager.Active.LocationDataFolder }\\{ FileName }"); } }
        [JsonIgnore]
        public uint RecordCount
        {
            get
            {
                return new BINDatabase(SettingsManager.Active.LocationDataFolder).GetTable(TableName).RecordCount;
            }
        }

        public override string ToString()
        {
            return string.Format("LocationFile({0}, '{1}')", DateTime, FileName);
        }
    }
}
