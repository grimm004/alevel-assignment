using Newtonsoft.Json;
using System.IO;

namespace LocationInterface.Utils
{
    public class ImageFile
    {
        public string FileName { get; set; }
        public double Multiplier { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        [JsonIgnore]
        public Vector2 Offset { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }

        [JsonIgnore]
        public string Identifier { get { return Path.GetFileNameWithoutExtension(FileName); } }

        [JsonIgnore]
        public bool Exists { get { return File.Exists($"{ SettingsManager.Active.ImageFolder }\\{ FileName }"); } }

        /// <summary>
        /// Get the hash code of the class
        /// </summary>
        /// <returns>the hash code as an XOR result of all the core members</returns>
        public override int GetHashCode()
        {
            // Return the XOR of all the hash codes of the core class properties
            return FileName.GetHashCode() ^ Multiplier.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
