using AnalysisSDK;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SharpMath2;
using System;
using System.IO;

namespace LocationInterface.Utils
{
    public class LocationMap : WpfGame
    {
        private ImageFile CurrentImageFile { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private IGraphicsDeviceService GraphicsDeviceManager { get; set; }
        private WpfKeyboard WpfKeyboard { get; set; }
        private WpfMouse WpfMouse { get; set; }

        private IMapper[] PluginMaps { get; set; }

        private MacPointCollection[] MacPointCollections { get; set; }

        private int KeyYOffset { get; set; }
        private Random Random { get; set; }
        private Camera Camera { get; set; }
        private KeyListener SKeyBind { get; set; }
        private Texture2D MapTexture { get; set; }
        private MapArea[] MapAreas { get; set; }
        
        private int pointRadius;
        private int PointRadius
        {
            get
            {
                return pointRadius;
            }
            set
            {
                pointRadius = value;
                // Check the radius is within the desired bounds
                if (pointRadius < 1) pointRadius = 1;
                if (pointRadius > 10) pointRadius = 10;

                int diameter = 2 * pointRadius;
                // Create an instance of the texture
                StandardContent.PointTexture = new Texture2D(GraphicsDevice, diameter, diameter);
                // Deine and initialize the array that will store the texture data
                Color[] circleData = new Color[diameter * diameter];
                int i = 0;
                // Loop through each x-value between the negitive radius and positive radius
                for (int x = -pointRadius; x < pointRadius; x++)
                    // Loop through each y-value between the negitive radius and positive radius
                    for (int y = -pointRadius; y < pointRadius; y++)
                        // If the sum of the squares of the current x and y values is less than the square of the radius (pythagoras)
                        if ((x * x) + (y * y) < pointRadius * pointRadius)
                            // Set the current pixel to be white (points are tinted a colour during runtime)
                            circleData[i++] = Color.White;
                        // Else set the colour of the position to not have an alpha (is transparent)
                        else circleData[i++] = new Color(0, 0, 0, 0);
                // Set the data in the texture instance
                StandardContent.PointTexture.SetData(circleData);
            }
        }

        public bool TimeBased { get; set; }
        
        private KeyListener DecreasePointSizeListener { get; set; }
        private KeyListener IncreasePointSizeListener { get; set; }
        
        /// <summary>
        /// Initialze the WpfGame instance along with import LocationMap items
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDeviceManager = new WpfGraphicsDeviceService(this);
            
            WpfKeyboard = new WpfKeyboard(this);
            WpfMouse = new WpfMouse(this);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera();
            
            CurrentImageFile = new ImageFile();
            
            PointRadius = 5;
            DecreasePointSizeListener = new KeyListener(Keys.C, () => PointRadius--);
            IncreasePointSizeListener = new KeyListener(Keys.V, () => PointRadius++);

            KeyYOffset = 20;
            
            TimeBased = false;

            Random = new Random();
            MacPointCollections = new MacPointCollection[0];

            // Create a KeyListener for the 'S' key (used for saving image metadata)
            SKeyBind = new KeyListener(Keys.S, SaveInfo);

            MapAreas = new MapArea[0];
            PluginMaps = new IMapper[0];

            StandardContent.Initialize(Content);
            foreach (MapperPlugin mapperPlugin in PluginManager.MapperPlugins)
                mapperPlugin.Mapper.Initialize(Content);
        }

        public void LoadPlugins(IMapper[] mapperPlugins)
        {
            PluginMaps = mapperPlugins;
        }

        public void UnloadPlugins()
        {
            PluginMaps = new IMapper[0];
        }

        /// <summary>
        /// Load an array of MacPointCollections
        /// </summary>
        /// <param name="points">The points to load</param>
        public void LoadPoints(MacPointCollection[] points)
        {
            MacPointCollections = points;
        }
        
        /// <summary>
        /// Load a map file
        /// </summary>
        /// <param name="selectedImageFile">The image file of the map to load</param>
        public void LoadMap(ImageFile selectedImageFile)
        {
            if (selectedImageFile != null)
            {
                CurrentImageFile = selectedImageFile;
                FileStream fileStream = new FileStream($"{ SettingsManager.Active.ImageFolder }\\{ selectedImageFile.FileName }", FileMode.Open);
                MapTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
                MapAreas = selectedImageFile.MapAreas;
            }
            else Console.WriteLine("Selected image file was null");
        }

        /// <summary>
        /// Called when the 'S' key is pressed
        /// </summary>
        protected void SaveInfo()
        {
            // If the control key is down, save the image index
            if (WpfKeyboard.GetState().IsKeyDown(Keys.LeftControl))
                App.ImageIndex.SaveIndex();
        }

        /// <summary>
        /// Update the location map state
        /// </summary>
        /// <param name="time">Information about timings since the previous update</param>
        protected override void Update(GameTime time)
        {
            // Fetch the mouse and keyboard states
            MouseState mouseState = WpfMouse.GetState();
            KeyboardState keyboardState = WpfKeyboard.GetState();

            // Update the binds
            SKeyBind.Update(keyboardState);
            DecreasePointSizeListener.Update(keyboardState);
            IncreasePointSizeListener.Update(keyboardState);
            // Update the controls
            bool shiftDown = keyboardState.IsKeyDown(Keys.LeftShift);
            if (keyboardState.IsKeyDown(Keys.A)) Camera.Move(shiftDown ? 20 : 5, 0);
            if (keyboardState.IsKeyDown(Keys.D)) Camera.Move(shiftDown ? -20 : -5, 0);
            if (keyboardState.IsKeyDown(Keys.W)) Camera.Move(0, shiftDown ? 20 : 5);
            if (keyboardState.IsKeyDown(Keys.S) && !keyboardState.IsKeyDown(Keys.LeftControl)) Camera.Move(0, shiftDown ? -20 : -5);
            if (keyboardState.IsKeyDown(Keys.R)) ScalePoints(new Vector2(shiftDown ? -.05f : -.001f, 0));
            if (keyboardState.IsKeyDown(Keys.Y)) ScalePoints(new Vector2(shiftDown ? +.05f : +.001f, 0));
            if (keyboardState.IsKeyDown(Keys.U)) ScalePoints(new Vector2(0, shiftDown ? -.05f : -.001f));
            if (keyboardState.IsKeyDown(Keys.J)) ScalePoints(new Vector2(0, shiftDown ? +.05f : +.001f));
            if (keyboardState.IsKeyDown(Keys.T)) TranslatePoints(0, shiftDown ? -4 : -1);
            if (keyboardState.IsKeyDown(Keys.F)) TranslatePoints(shiftDown ? -4 : -1, 0);
            if (keyboardState.IsKeyDown(Keys.G)) TranslatePoints(0, shiftDown ? +4 : +1);
            if (keyboardState.IsKeyDown(Keys.H)) TranslatePoints(shiftDown ? +4 : +1, 0);
            if (keyboardState.IsKeyDown(Keys.Z)) KeyYOffset -= shiftDown ? +20 : +5;
            if (keyboardState.IsKeyDown(Keys.X)) KeyYOffset += shiftDown ? +20 : +5;
            if (keyboardState.IsKeyDown(Keys.Q)) Camera.Scale -= shiftDown ? .2f : .01f;
            if (keyboardState.IsKeyDown(Keys.E)) Camera.Scale += shiftDown ? .2f : .01f;

            if (Camera.Scale < .01f) Camera.Scale = .01f;
            else if (Camera.Scale > 10) Camera.Scale = 10;

            if (CurrentImageFile.FileName != "")
            {
                foreach (MacPointCollection macPointCollection in MacPointCollections)
                    foreach (LocationPoint locationPoint in macPointCollection.MapLocationPoints[CurrentImageFile.FileName].Points)
                        locationPoint.InArea = false;

                foreach (MapArea area in MapAreas)
                    if (area.Polygon != null)
                        foreach (MacPointCollection macPointCollection in MacPointCollections)
                            foreach (LocationPoint locationPoint in macPointCollection.MapLocationPoints[CurrentImageFile.FileName].Points)
                                if (!locationPoint.InArea)
                                    locationPoint.InArea = Polygon2.Contains(area.Polygon, Vector2.Zero, Rotation2.Zero, locationPoint, false);
            }

            foreach (IMapper mapperPlugin in PluginMaps) mapperPlugin.Update(time);
        }

        /// <summary>
        /// Change the scale of all the points
        /// </summary>
        /// <param name="change">The scale to increment by</param>
        public void ScalePoints(Vector2 change)
        {
            // Increment the stored multiplyer for the current image file
            CurrentImageFile.Scale += new Vector2(CurrentImageFile.FlipHorizontal ? -1 : 1, CurrentImageFile.FlipVertical ? -1 : 1) * change;
        }

        /// <summary>
        /// Change the offset of all the points
        /// </summary>
        /// <param name="x">The number of pixels to offset the points by on the x axis</param>
        /// <param name="y">The number of pixels to offset the points by on the y axis</param>
        public void TranslatePoints(float x, float y)
        {
            // Increment the image file offset by the desired offset
            CurrentImageFile.Offset += new Vector2(CurrentImageFile.FlipHorizontal ? -x : x, CurrentImageFile.FlipVertical ? -y : y);
        }
        
        /// <summary>
        /// Draw the location map state
        /// </summary>
        /// <param name="time">Information about time since the previous draw</param>
        protected override void Draw(GameTime time)
        {
            // Draw a white background
            GraphicsDevice.Clear(Color.White);
            
            // Begin drawing all the 2D sprites at the camera's translation
            SpriteBatch.Begin(transformMatrix: Camera.Transformation);
            // Draw the points
            DrawPoints();
            DrawBounds();
            SpriteBatch.End();
            
            // Begin drawing all the static 2D sprites
            SpriteBatch.Begin();
            // Draw the MAC key
            DrawKey();
            SpriteBatch.End();

            foreach (IMapper mapperPlugin in PluginMaps) mapperPlugin.Draw(time, SpriteBatch);
        }

        private void DrawHeatPoint(HeatPoint HeatPoint, int Radius)
        {
            //// Create points generic list of points to hold circumference points
            //List<Point> CircumferencePointsList = new List<Point>();
            //// Create an empty point to predefine the point struct used in the circumference loop
            //Point CircumferencePoint;
            //// Create an empty array that will be populated with points from the generic list
            //Point[] CircumferencePointsArray;
            //// Calculate ratio to scale byte intensity range from 0-255 to 0-1
            //float fRatio = 1F / Byte.MaxValue;
            //// Precalulate half of byte max value
            //byte bHalf = Byte.MaxValue / 2;
            //// Flip intensity on it's center value from low-high to high-low
            //int iIntensity = (byte)(HeatPoint.Intensity - ((HeatPoint.Intensity - bHalf) * 2));
            //// Store scaled and flipped intensity value for use with gradient center location
            //float fIntensity = iIntensity * fRatio;
            //// Loop through all angles of a circle
            //// Define loop variable as a double to prevent casting in each iteration
            //// Iterate through loop on 10 degree deltas, this can change to improve performance
            //for (double i = 0; i <= 360; i += 10)
            //{
            //    // Replace last iteration point with new empty point struct
            //    CircumferencePoint = new Point();
            //    // Plot new point on the circumference of a circle of the defined radius
            //    // Using the point coordinates, radius, and angle
            //    // Calculate the position of this iterations point on the circle
            //    CircumferencePoint.X = Convert.ToInt32(HeatPoint.X + Radius * Math.Cos(ConvertDegreesToRadians(i)));
            //    CircumferencePoint.Y = Convert.ToInt32(HeatPoint.Y + Radius * Math.Sin(ConvertDegreesToRadians(i)));
            //    // Add newly plotted circumference point to generic point list
            //    CircumferencePointsList.Add(CircumferencePoint);
            //}
            //// Populate empty points system array from generic points array list
            //// Do this to satisfy the datatype of the PathGradientBrush and FillPolygon methods
            //CircumferencePointsArray = CircumferencePointsList.ToArray();
            //// Create new PathGradientBrush to create a radial gradient using the circumference points
            //PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);
            //// Create new color blend to tell the PathGradientBrush what colors to use and where to put them
            //ColorBlend GradientSpecifications = new ColorBlend(3);
            //// Define positions of gradient colors, use intesity to adjust the middle color to
            //// show more mask or less mask
            //GradientSpecifications.Positions = new float[3] { 0, fIntensity, 1 };
            //// Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            //GradientSpecifications.Colors = new Color[3]
            //{
            //    Color.FromArgb(0, Color.White),
            //    Color.FromArgb(HeatPoint.Intensity, Color.Black),
            //    Color.FromArgb(HeatPoint.Intensity, Color.Black)
            //};
            //// Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            //GradientShaper.InterpolationColors = GradientSpecifications;
            //// Draw polygon (circle) using our point array and gradient brush
            //Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }

        /// <summary>
        /// Draw the current location points
        /// </summary>
        protected void DrawPoints()
        {
            // If there is a loaded map texure, draw it
            if (MapTexture != null) SpriteBatch.Draw(MapTexture, Vector2.Zero);
            // Loop through the macpointcollections
            foreach (MacPointCollection macPointCollection in MacPointCollections)
                if (macPointCollection.MapLocationPoints.ContainsKey(CurrentImageFile.FileName))
                    // Loop through each macpoint in the current macpointcollection
                    foreach (LocationPoint locationPoint in macPointCollection.MapLocationPoints[CurrentImageFile.FileName].Points)
                        // Draw the point with the desired colour with its offset and multiplier
                        SpriteBatch.Draw(StandardContent.PointTexture, new Vector2(CurrentImageFile.FlipHorizontal ? MapTexture.Width : 0, CurrentImageFile.FlipVertical ? MapTexture.Height : 0)
                            + (new Vector2(CurrentImageFile.FlipHorizontal ? -1 : 1, CurrentImageFile.FlipVertical ? -1 : 1) * (CurrentImageFile.Offset +
                            (CurrentImageFile.Scale * locationPoint))), null, macPointCollection.Colour,
                            0f, new Vector2(PointRadius / 2), 1f, SpriteEffects.None, 0);
        }

        protected void DrawBounds()
        {
            foreach (MapArea mapArea in MapAreas)
                mapArea.Draw(SpriteBatch, CurrentImageFile.FlipHorizontal, CurrentImageFile.FlipVertical, MapTexture.Width, MapTexture.Height, CurrentImageFile.Offset, CurrentImageFile.Scale);
        }

        /// <summary>
        /// Draw the MAC info key
        /// </summary>
        protected void DrawKey()
        {
            // Define a position storage vector
            Vector2 position = new Vector2(20, 20 - KeyYOffset);
            // Loop through each macpointcollection
            for (int i = 0; i < MacPointCollections.Length; i++)
                if (MacPointCollections[i].MapLocationPoints.ContainsKey(CurrentImageFile.FileName))
                {
                    // Draw a point in the desired colour
                    SpriteBatch.Draw(StandardContent.PointTexture, position, null,
                        MacPointCollections[i].Colour, 0f, new Vector2(PointRadius / 2),
                        1f, SpriteEffects.None, 0);
                    // Draw the MAC address
                    SpriteBatch.DrawString(StandardContent.Font, MacPointCollections[i].Address,
                        position + new Vector2(PointRadius * 2, PointRadius / -2), Color.Black);
                    // If in time based mode
                    if (TimeBased)
                    {
                        // Draw the time of the point being displayed
                        SpriteBatch.DrawString(StandardContent.Font, MacPointCollections[i].MapLocationPoints[CurrentImageFile.FileName].Points.Count > 0
                            ? MacPointCollections[i].MapLocationPoints[CurrentImageFile.FileName].Points[0].Time.ToString(@"hh\:mm\:ss") :
                            "No Points", position + new Vector2(120 + (PointRadius * 2),
                            PointRadius / -2), Color.Black);
                        // If there is a point being played, draw its location node
                        if (MacPointCollections[i].MapLocationPoints[CurrentImageFile.FileName].Points.Count > 0)
                            SpriteBatch.DrawString(StandardContent.Font, MacPointCollections[i].MapLocationPoints[CurrentImageFile.FileName].Points[0].Node,
                                position + new Vector2(175 + (PointRadius * 2),
                                PointRadius / -2), Color.Black);
                    }
                    // Increment the y position
                    position.Y += 20;
                }
        }
    }

    public struct HeatPoint
    {
        public int X;
        public int Y;
        public byte Intensity;
        public HeatPoint(int iX, int iY, byte bIntensity)
        {
            X = iX;
            Y = iY;
            Intensity = bIntensity;
        }
    }
}
