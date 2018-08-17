using Microsoft.Xna.Framework;
using System;

namespace LocationInterface.Utils
{
    public class LocationPoint
    {
        public Vector2 Point { get; set; }
        public TimeSpan Time { get; set; }
        public string Node { get; set; }
        public bool InArea { get; set; }

        public LocationPoint()
        {
            Point = Vector2.Zero;
            Time = new TimeSpan();
            Node = "";
            InArea = true;
        }

        /// <summary>
        /// Implicitly convert to a Vector2
        /// </summary>
        /// <param name="point">the point to convert</param>
        public static implicit operator Vector2(LocationPoint point)
        {
            return point.Point;
        }

        /// <summary>
        /// Implicitly convert to a TimeSpan
        /// </summary>
        /// <param name="point">the point to convert</param>
        public static implicit operator TimeSpan(LocationPoint point)
        {
            return point.Time;
        }
    }
}