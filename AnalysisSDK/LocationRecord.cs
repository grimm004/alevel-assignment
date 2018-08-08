using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AnalysisSDK
{
    public class LocationRecord
    {
        public string MAC { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Deck { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
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
