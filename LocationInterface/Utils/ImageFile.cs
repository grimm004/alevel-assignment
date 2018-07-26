using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;

namespace LocationInterface.Utils
{
    public class ImageFile
    {
        public string FileName { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Initialize an imagefile instacne
        /// </summary>
        public ImageFile()
        {
            FileName = "";
            Scale = new Vector2(1);
            Offset = Vector2.Zero;
        }

        /// <summary>
        /// Initialze an imagefile instance
        /// </summary>
        /// <param name="fileName">The name of the image file</param>
        /// <param name="multiplier">The point scale multiplier of the image file</param>
        /// <param name="offset">The point offset of the image file</param>
        public ImageFile(string fileName, Vector2 scale, Vector2 offset)
        {
            FileName = fileName;
            Scale = scale;
            Offset = offset;
        }

        [JsonIgnore]
        public string Identifier { get { return Path.GetFileNameWithoutExtension(FileName); } }

        [JsonIgnore]
        public bool Exists { get { return File.Exists($"{ SettingsManager.Active.ImageFolder }\\{ FileName }"); } }
        
        public ImageFileReference GetReference()
        {
            return new ImageFileReference(FileName);
        }

        /// <summary>
        /// Get the hash code of the class
        /// </summary>
        /// <returns>the hash code as an XOR result of all the core members</returns>
        public override int GetHashCode()
        {
            // Return the XOR of all the hash codes of the core class properties
            return FileName.GetHashCode() ^ Scale.GetHashCode() ^ Offset.GetHashCode();
        }
        public override string ToString()
        {
            return $"ImageFile('{ FileName }', { Scale.ToString() }, { Offset })";
        }
    }

    public class ImageFileReference
    {
        public string FileName { get; set; }

        [JsonIgnore]
        public string Identifier { get { return Path.GetFileNameWithoutExtension(FileName); } }

        /// <summary>
        /// Initialize an ImageFileReference instacne
        /// </summary>
        public ImageFileReference()
        {
            FileName = "";
        }

        /// <summary>
        /// Initialize an ImageFileReference instacne
        /// </summary>
        public ImageFileReference(string fileName)
        {
            FileName = fileName;
        }
    }
}
