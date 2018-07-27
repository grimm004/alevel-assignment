using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LocationInterface.Utils
{
    public class MacPointCollection
    {
        public string Address { get; set; }
        public Color Colour { get; set; }
        public Dictionary<string, List<LocationPoint>> MapLocationPoints { get; set; }

        /// <summary>
        /// Initialze a MacPointCollection
        /// </summary>
        public MacPointCollection()
        {
            Address = "";
            Colour = Color.Black;
            MapLocationPoints = new Dictionary<string, List<LocationPoint>>();
            foreach (ImageFile imageFile in App.ImageIndex.ImageFiles)
                MapLocationPoints.Add(imageFile.DataReference, new List<LocationPoint>());
        }
    }
}
