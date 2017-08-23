﻿using System;

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
    }
}