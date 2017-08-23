using System;
using System.IO;
using Newtonsoft.Json;

namespace LocationInterface.Utils
{
    public class LocationDataFile
    {
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }

        [JsonIgnore]
        public string TableName { get { return Path.GetFileNameWithoutExtension(FileName); } }
        [JsonIgnore]
        public bool Exists { get { return File.Exists(@"LocationData\" + FileName); } }

        public override string ToString()
        {
            return string.Format("LocationFile({0}, '{1}')", DateTime, FileName);
        }
    }
}
