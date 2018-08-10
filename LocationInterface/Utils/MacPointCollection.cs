using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LocationInterface.Utils
{
    public class MacPointCollection
    {
        public string Address { get; set; }
        public Color Colour { get; set; }
        public Dictionary<string, MapLocationPoint> MapLocationPoints { get; set; }

        /// <summary>
        /// Initialze a MacPointCollection
        /// </summary>
        public MacPointCollection()
        {
            Address = "";
            Colour = Color.Black;
            MapLocationPoints = new Dictionary<string, MapLocationPoint>();
            foreach (ImageFile imageFile in App.ImageIndex.ImageFiles)
                if (!MapLocationPoints.ContainsKey(imageFile.DataReference))
                    MapLocationPoints.Add(imageFile.FileName, new MapLocationPoint(imageFile.DataReference, new List<LocationPoint>()));
        }
    }

    public class MapLocationPoint
    {
        public string LocationReference { get; set; }
        public List<LocationPoint> Points { get; set; }

        public MapLocationPoint(string dataReference, List<LocationPoint> locationPoints)
        {
            LocationReference = dataReference;
            Points = locationPoints;
        }
    }
}
