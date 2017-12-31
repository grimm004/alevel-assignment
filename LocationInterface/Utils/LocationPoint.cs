using Microsoft.Xna.Framework;
using System;

namespace LocationInterface.Utils
{
    public class LocationPoint
    {
        public Vector2 Point { get; set; }
        public TimeSpan Time { get; set; }

        public static implicit operator Vector2(LocationPoint point)
        {
            return point.Point;
        }

        public static implicit operator TimeSpan(LocationPoint point)
        {
            return point.Time;
        }
    }
}