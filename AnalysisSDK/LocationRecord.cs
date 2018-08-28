using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DatabaseManagerLibrary;
using System;

namespace AnalysisSDK
{
    public class LocationRecord
    {
        [FieldIdentifier("mac")]
        public string MAC { get; set; }
        [FieldIdentifier("start_ts")]
        public DateTime Date { get; set; }
        [FieldIdentifier("floor")]
        public string Floor { get; set; }
        [FieldIdentifier("xcoords")]
        public double X { get; set; }
        [FieldIdentifier("ycoords")]
        public double Y { get; set; }

        [Ignore]
        public string Area { get; set; }

        public override string ToString()
        {
            return $"{ MAC }, { Date }, { Floor }, ({ X }, { Y })";
        }
    }

    public static class StandardContent
    {
        public static SpriteFont Font { get; private set; }
        public static Texture2D PointTexture { get; set; }

        public static void Initialize(ContentManager content)
        {
            // Load a pre-compiled font
            Font = content.Load<SpriteFont>("Font");
        }
    }
}
