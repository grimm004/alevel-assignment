using Microsoft.Xna.Framework;
using System;

namespace LocationInterface.Utils
{
    public class LocationRecord
    {
        public string MAC { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Deck { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Convert to a Vector2
        /// </summary>
        /// <param name="record">The record to convert from</param>
        public static implicit operator Vector2(LocationRecord record)
        {
            return new Vector2((float)record.X, (float)record.Y);
        }

        /// <summary>
        /// Convert to a LocationPoint
        /// </summary>
        /// <param name="record">The record to convert from</param>
        public static implicit operator LocationPoint(LocationRecord record)
        {
            return new LocationPoint
            {
                Point = record,
                Time = record.Date.TimeOfDay,
                Node = record.Location,
            };
        }
    }
}
