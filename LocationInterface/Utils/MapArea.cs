using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SharpMath2;

//Count,Name,FLOOR#,POI#,POI_CAT,BUILDING#,VISIBILTY,End X,End Y,Start X,Start Y

namespace LocationInterface.Utils
{
    public class MapAreaFile
    {
        public string FileName { get; private set; }

        public MapAreaFile(string fileName)
        {
            FileName = fileName;
        }

        public MapArea[] LoadAreas()
        {
            List<MapArea> areas = new List<MapArea>();

            using (StreamReader reader = new StreamReader(FileName))
            {
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                string infoLine;
                while (!reader.EndOfStream && (infoLine = reader.ReadLine()) != null && infoLine != "")
                    areas.Add(new MapArea(infoLine, reader));
            }
            
            return areas.ToArray();
        }
    }

    public class MapArea
    {
        public string Identifier { get; private set; }
        public string Name { get; private set; }
        public Color Colour { get; }
        public List<Line> Bounds { get; }
        public Polygon2 Polygon { get; }

        public MapArea(string areaInformationLine, StreamReader reader)
        {
            Colour = Color.Red;
            if (areaInformationLine != null)
            {
                string[] areaInformation = areaInformationLine.Split(',');
                Identifier = areaInformation[3];
                Name = areaInformation[4];

                Bounds = new List<Line>();
                string positionLine;
                while ((positionLine = reader.ReadLine()) != null && positionLine != "")
                {
                    string[] positionInformation = positionLine.Split(',');
                    if (float.TryParse(positionInformation[9], out float xStart) && float.TryParse(positionInformation[10], out float yStart) &&
                        float.TryParse(positionInformation[7], out float xEnd)   && float.TryParse(positionInformation[8], out float yEnd))
                        Bounds.Add(new Line(xStart, yStart, xEnd, yEnd));
                    else break;
                }

                Vector2[] points = Bounds.Select(b => b.Start).ToArray();
                Polygon = points.Length > 0 ? new Polygon2(points) : null;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool fH, bool fV, int w, int h, Vector2 offset, Vector2 scale)
        {
            foreach (Line line in Bounds)
                line.Draw(spriteBatch, Color.Red, fH, fV, w, h, offset, scale);
        }
    }

    public class Line
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        public Line()
        {
            Start = Vector2.Zero;
            End = Vector2.Zero;
        }

        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public Line(float xStart, float yStart, float xEnd, float yEnd)
        {
            Start = new Vector2(xStart, yStart);
            End = new Vector2(xEnd, yEnd);
        }

        public void Draw(SpriteBatch spriteBatch, Color colour, bool fH, bool fV, int w, int h, Vector2 offset, Vector2 scale)
        {
            spriteBatch.DrawLine(FlipPoint(fH, fV, w, h, offset, scale, Start), FlipPoint(fH, fV, w, h, offset, scale, End), colour, 2);
        }

        private Vector2 FlipPoint(bool fH, bool fV, int w, int h, Vector2 offset, Vector2 scale, Vector2 point)
        {
            return new Vector2(fH ? w : 0, fV ? h : 0) + (new Vector2(fH ? -1 : 1, fV ? -1 : 1) * (offset + (scale * point)));
        }
    }
}
