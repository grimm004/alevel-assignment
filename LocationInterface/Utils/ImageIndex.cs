using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace LocationInterface.Utils
{
    public class ImageIndex
    {
        public List<ImageFile> ImageFiles { get; set; }
        [JsonIgnore]
        public List<ImageFileReference> ImageFileReferences { get; set; }

        /// <summary>
        /// Initialise the ImageIndex
        /// </summary>
        public ImageIndex()
        {
            ImageFiles = new List<ImageFile>();
            ImageFileReferences = new List<ImageFileReference>();
        }

        /// <summary>
        /// Load the image index from the file
        /// </summary>
        /// <returns>The loaded ImageIndex instance</returns>
        public static ImageIndex LoadIndex()
        {
            // Read the data in the image index files, convert it to a string, then deserialize to an ImageIndex object with JsonConvert
            return JsonConvert.DeserializeObject<ImageIndex>(Encoding.UTF8.GetString(File.ReadAllBytes($"{ SettingsManager.Active.ImageFolder }\\index.json")));
        }

        /// <summary>
        /// Save the current dataindex instance to the index file
        /// </summary>
        public void SaveIndex()
        {
            // Serialize the current ImageIndex instance to a string, then write it to the index file
            File.WriteAllBytes($"{ SettingsManager.Active.ImageFolder }\\index.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented)));
        }

        public ImageFile GetImageFile(ImageFileReference reference)
        {
            for (int i = 0; i < ImageFiles.Count; i++)
                if (ImageFiles[i].FileName == reference.FileName) return ImageFiles[i];
            return null;
        }

        /// <summary>
        /// Scan through the images folder and add new files to the index
        /// </summary>
        public void ScanForFiles()
        {
            // Loop through each file with the file extention '.bmp' (bitmap) in the image folder
            foreach (string fileName in Directory.GetFiles(SettingsManager.Active.ImageFolder, "*.bmp"))
                // If the image file is not in the index
                if (!ImageFileExists(Path.GetFileName(fileName)))
                    // Add the file to the index
                    ImageFiles.Add(new ImageFile(Path.GetFileName(fileName), new Vector2(1), Vector2.Zero, ""));
            // Reorder the index alphabetically
            ImageFiles = ImageFiles.OrderBy(imageFile => imageFile.Identifier).ToList();
            LoadReferences();
        }

        public void LoadReferences()
        {
            ImageFileReferences.Clear();
            foreach (ImageFile imageFile in ImageFiles)
                ImageFileReferences.Add(imageFile.GetReference());
        }

        /// <summary>
        /// Get a serch for and get a data file object
        /// </summary>
        /// <param name="imageIdentifier">The identifier to search for</param>
        /// <returns>the image file object if found, null if not found</returns>
        public ImageFile GetDataFile(string imageIdentifier)
        {
            // Loop through each image file object in the image file list, if the current image identifier equals the image identifier return the current image object
            foreach (ImageFile currentImageFile in ImageFiles) if (currentImageFile.Identifier == imageIdentifier) return currentImageFile;
            // Return null
            return null;
        }

        /// <summary>
        /// Check if an image file exists
        /// </summary>
        /// <param name="fileName">The name of the file to check</param>
        /// <returns>true if the image file exists</returns>
        public bool ImageFileExists(string fileName)
        {
            // Loop through each image file in the image files list, if the current image file name equals the image file name return true
            foreach (ImageFile currentImageFile in ImageFiles) if (currentImageFile.FileName == fileName) return true;
            // Return false
            return false;
        }
        /// <summary>
        /// Check if an image file exists
        /// </summary>
        /// <param name="imageFile">The image file to check</param>
        /// <returns>true if the image file exists</returns>
        public bool ImageFileExists(ImageFile imageFile)
        {
            // Loop through each image file in the image files list, if the current image file hash code equals the image file hash code return true
            foreach (ImageFile currentImageFile in ImageFiles) if (currentImageFile.GetHashCode() == imageFile.GetHashCode()) return true;
            // Return false
            return false;
        }

        /// <summary>
        /// Verify the existance of the image files
        /// </summary>
        public void VerifyImageFiles()
        {
            // Loop through each image file and if it does not exist remove it from the index
            for (int i = ImageFiles.Count - 1; i >= 0; i--) if (!ImageFiles[i].Exists) ImageFiles.RemoveAt(i);
            LoadReferences();
            // Save the data index
            SaveIndex();
        }
    }
}
