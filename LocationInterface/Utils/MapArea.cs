using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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
        public Color Colour { get; set; }
        public List<Line> Bounds { get; private set; }
        public Polygon Polygon { get; private set; }

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

                Polygon = new Polygon();
                Polygon.Points.AddRange(Bounds.Select(b => b.Start));
                Polygon.BuildEdges();
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

    public class Polygon
    {
        public void BuildEdges()
        {
            Vector2 p1;
            Vector2 p2;
            Edges.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                p1 = Points[i];
                if (i + 1 >= Points.Count) p2 = Points[0];
                else p2 = Points[i + 1];
                Edges.Add(p2 - p1);
            }
        }

        public List<Vector2> Edges { get; } = new List<Vector2>();
        public List<Vector2> Points { get; } = new List<Vector2>();

        public Vector2 Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < Points.Count; i++)
                {
                    totalX += Points[i].X;
                    totalY += Points[i].Y;
                }

                return new Vector2(totalX / (float)Points.Count, totalY / (float)Points.Count);
            }
        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Vector2 p = Points[i];
                Points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }
    }

    public struct PolygonCollisionResult
    {
        public bool WillIntersect { get; set; }
        public bool Intersecting { get; set; }
        public Vector2 MPV { get; set; }
    }

    public static class PolygonCollisionDetection
    {
        // Check if polygon A is going to collide with polygon B for the given velocity
        public static PolygonCollisionResult PolygonCollision(this Polygon polygonA, Polygon polygonB, Vector2 velocity)
        {
            PolygonCollisionResult result = new PolygonCollisionResult
            {
                Intersecting = true,
                WillIntersect = true
            };

            int edgeCountA = polygonA.Edges.Count;
            int edgeCountB = polygonB.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA) edge = polygonA.Edges[edgeIndex];
                else edge = polygonB.Edges[edgeIndex - edgeCountA];

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0, minB = 0, maxA = 0, maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0) result.Intersecting = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                float velocityProjection = axis.DotProduct(velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0) minA += velocityProjection;
                else maxA += velocityProjection;

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersecting && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    if ((polygonA.Center - polygonB.Center).DotProduct(translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect) result.MPV = translationAxis * minIntervalDistance;

            return result;
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB) return minB - maxA;
            else return minA - maxB;
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        private static void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = axis.DotProduct(polygon.Points[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = polygon.Points[i].DotProduct(axis);
                if (d < min) min = d;
                else if (d > max) max = d;
            }
        }
    }

    public static class VectorExtensions
    {
        public static float DotProduct(this Vector2 vec, Vector2 vector)
        {
            return (vec.X * vector.X) + (vec.Y * vector.Y);
        }
    }
}
