using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;

namespace LocationInterface.Utils
{
    public class ImageFile
    {
        public string FileName { get; set; }
        public string DataReference { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Offset { get; set; }
        public string AreaFileName { get; set; }
        public bool FlipHorizontal { get; set; }
        public bool FlipVertical { get; set; }

        /// <summary>
        /// Initialize an imagefile instacne
        /// </summary>
        public ImageFile()
        {
            FileName = "";
            AreaFileName = "";
            Scale = new Vector2(1);
            Offset = Vector2.Zero;
            DataReference = "None";
            FlipHorizontal = false;
            FlipVertical = false;
        }
        
        /// <summary>
        /// Initialze an imagefile instance
        /// </summary>
        /// <param name="fileName">The name of the image file</param>
        /// <param name="multiplier">The point scale multiplier of the image file</param>
        /// <param name="offset">The point offset of the image file</param>
        public ImageFile(string fileName, Vector2 scale, Vector2 offset, string areaFileName)
        {
            FileName = fileName;
            Scale = scale;
            Offset = offset;
            AreaFileName = areaFileName;
            DataReference = Path.GetFileNameWithoutExtension(FileName);
            FlipHorizontal = false;
            FlipVertical = false;
        }

        [JsonIgnore]
        public bool Exists { get { return File.Exists(FullFileName); } }

        [JsonIgnore]
        public string FullFileName { get { return $"{ SettingsManager.Active.ImageFolder }\\{ FileName }"; } }

        [JsonIgnore]
        public MapArea[] MapAreas
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AreaFileName) || !File.Exists(AreaFileName))
                {
                    System.Console.WriteLine($"Could not find area file '{ AreaFileName }' for '{ FileName }'.");
                    return new MapArea[0];
                }
                else return new MapAreaFile(AreaFileName).LoadAreas();
            }
        }
        
        public ImageFileReference GetReference()
        {
            return new ImageFileReference(FileName, DataReference);
        }

        public void Delete()
        {
            if (Exists) File.Delete(FullFileName);
        }

        /// <summary>
        /// Get the hash code of the class
        /// </summary>
        /// <returns>the hash code as an XOR result of all the core members</returns>
        public override int GetHashCode()
        {
            // Return the XOR of all the hash codes of the core class properties
            return FileName.GetHashCode() ^ Scale.GetHashCode() ^ Offset.GetHashCode() ^ DataReference.GetHashCode() ^ AreaFileName.GetHashCode() ^ FlipHorizontal.GetHashCode() ^ FlipVertical.GetHashCode();
        }
        public override string ToString()
        {
            return $"ImageFile('{ FileName }', { Scale.ToString() }, { Offset })";
        }
    }

    public class ImageFileReference
    {
        public string FileName { get; set; }
        public string DataReference { get; set; }

        /// <summary>
        /// Initialize an ImageFileReference instacne
        /// </summary>
        public ImageFileReference()
        {
            FileName = "";
            DataReference = "None";
        }

        /// <summary>
        /// Initialize an ImageFileReference instacne
        /// </summary>
        public ImageFileReference(string fileName, string dataReference)
        {
            FileName = fileName;
            DataReference = dataReference;
        }
    }
}
