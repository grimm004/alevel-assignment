using System;
using System.IO;

namespace LocationInterface.Utils
{
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
