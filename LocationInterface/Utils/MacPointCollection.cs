using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LocationInterface.Utils
{
    public class MacPointCollection
    {
        public string Address { get; set; }
        public Color Colour { get; set; }
        public List<LocationPoint> MacPoints { get; set; }

        /// <summary>
        /// Initialze a MacPointCollection
        /// </summary>
        public MacPointCollection()
        {
            Address = "";
            Colour = Color.Black;
            MacPoints = new List<LocationPoint>();
        }
    }
}
