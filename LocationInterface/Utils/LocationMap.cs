﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using System.IO;

namespace LocationInterface.Utils
{
    public class LocationMap : WpfGame
    {
        private ImageFile SelectedImageFile { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private IGraphicsDeviceService GraphicsDeviceManager { get; set; }
        private WpfKeyboard WpfKeyboard { get; set; }
        private WpfMouse WpfMouse { get; set; }

        private MacPointCollection[] MacPointCollections { get; set; } = new MacPointCollection[0];

        private Texture2D PointTexture { get; set; }
        private Random Random { get; set; }
        private Camera Camera { get; set; }
        private KeyListener SKeyBind { get; set; }
        private Texture2D MapTexture { get; set; }

        public bool TimeBased { get; set; }

        private SpriteFont Font { get; set; }

        /// <summary>
        /// Initialze the WpfGame instance along with import LocationMap items
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDeviceManager = new WpfGraphicsDeviceService(this);

            WpfKeyboard = new WpfKeyboard(this);
            WpfMouse = new WpfMouse(this);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera();

            SelectedImageFile = new ImageFile();
            
            base.Initialize();

            // Mathematically create a circle texture of the defined radius
            int radius = 5, diameter = radius * 2;
            
            // Create an instance of the texture
            PointTexture = new Texture2D(GraphicsDevice, diameter, diameter);
            // Deine and initialize the array that will store the texture data
            Color[] circleData = new Color[diameter * diameter];
            int i = 0;
            // Loop through each x-value between the negitive radius and positive radius
            for (int x = -radius; x < radius; x++)
                // Loop through each y-value between the negitive radius and positive radius
                for (int y = -radius; y < radius; y++)
                    // If the sum of the squares of the current x and y values is less than the square of the radius (pythagoras)
                    if ((x * x) + (y * y) < radius * radius)
                        // Set the current pixel to be white (points are tinted a colour during runtime)
                        circleData[i++] = Color.White;
                    // Else set the colour of the position to not have an alpha (is transparent)
                    else circleData[i++] = new Color(0, 0, 0, 0);
            // Set the data in the texture instance
            PointTexture.SetData(circleData);
            TimeBased = false;

            Random = new Random();
            MacPointCollections = new MacPointCollection[0];

            // Load a pre-compiled font
            Font = Content.Load<SpriteFont>("Font");

            // Create a KeyListener for the 'S' key (used for saving image metadata)
            SKeyBind = new KeyListener(Keys.S, SaveInfo);
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
            SelectedImageFile = selectedImageFile;
            FileStream fileStream = new FileStream($"{ SettingsManager.Active.ImageFolder }\\{ selectedImageFile.FileName }", FileMode.Open);
            // Dynamically produce the map texture to be rendered in the background
            MapTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
            fileStream.Dispose();
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
            var mouseState = WpfMouse.GetState();
            var keyboardState = WpfKeyboard.GetState();

            // Update the bind
            SKeyBind.Update(keyboardState);
            // Update the controls
            bool shiftDown = keyboardState.IsKeyDown(Keys.LeftShift);
            if (keyboardState.IsKeyDown(Keys.A)) Camera.Move(5, 0);
            if (keyboardState.IsKeyDown(Keys.D)) Camera.Move(-5, 0);
            if (keyboardState.IsKeyDown(Keys.W)) Camera.Move(0, 5);
            if (keyboardState.IsKeyDown(Keys.S) && !keyboardState.IsKeyDown(Keys.LeftControl)) Camera.Move(0, -5);
            if (keyboardState.IsKeyDown(Keys.R)) ScalePoints(shiftDown ? -.05f : -.001f);
            if (keyboardState.IsKeyDown(Keys.Y)) ScalePoints(shiftDown ? +.05f : +.001f);
            if (keyboardState.IsKeyDown(Keys.T)) TranslatePoints(0, shiftDown ? -4 : -1);
            if (keyboardState.IsKeyDown(Keys.F)) TranslatePoints(shiftDown ? -4 : -1, 0);
            if (keyboardState.IsKeyDown(Keys.G)) TranslatePoints(0, shiftDown ? +4 : +1);
            if (keyboardState.IsKeyDown(Keys.H)) TranslatePoints(shiftDown ? +4 : +1, 0);
        }

        /// <summary>
        /// Change the scale of all the points
        /// </summary>
        /// <param name="change">The scale to increment by</param>
        public void ScalePoints(float change)
        {
            // Increment the stored multiplyer for the current image file
            SelectedImageFile.Multiplier += change;
        }

        /// <summary>
        /// Change the offset of all the points
        /// </summary>
        /// <param name="x">The number of pixels to offset the points by on the x axis</param>
        /// <param name="y">The number of pixels to offset the points by on the y axis</param>
        public void TranslatePoints(float x, float y)
        {
            // Increment the image file offset by the desired offset
            SelectedImageFile.Offset += new Vector2(x, y);
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
            SpriteBatch.End();

            // Begin drawing all the static 2D sprites
            SpriteBatch.Begin();
            // Draw the MAC key
            DrawKey();
            SpriteBatch.End();
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
                // Loop through each macpoint in the current macpointcollection
                foreach (Vector2 macPoint in macPointCollection.MacPoints)
                    // Draw the point with the desired colour with its offset and multiplier
                    SpriteBatch.Draw(PointTexture, SelectedImageFile.Offset + (SelectedImageFile.Multiplier * macPoint), null, macPointCollection.Colour, 0f, new Vector2(5), 1f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw the MAC info key
        /// </summary>
        protected void DrawKey()
        {
            // Define a position storage vector
            Vector2 position = new Vector2(20, 20);
            // Loop through each macpointcollection
            for (int i = 0; i < MacPointCollections.Length; i++)
            {
                // Draw a point in the desired colour
                SpriteBatch.Draw(PointTexture, position, null, MacPointCollections[i].Colour, 0f, new Vector2(5), 1f, SpriteEffects.None, 0);
                // Draw the MAC address
                SpriteBatch.DrawString(Font, MacPointCollections[i].Address, position + new Vector2(10, -5), Color.Black);
                // If in time based mode
                if (TimeBased)
                {
                    // Draw the time of the point being displayed
                    SpriteBatch.DrawString(Font, MacPointCollections[i].MacPoints.Count > 0 ? MacPointCollections[i].MacPoints[0].Time.ToString(@"hh\:mm\:ss") : "No Points", position + new Vector2(125, -5), Color.Black);
                    // If there is a point being played, draw its location node
                    if (MacPointCollections[i].MacPoints.Count > 0) SpriteBatch.DrawString(Font, MacPointCollections[i].MacPoints[0].Node, position + new Vector2(180, -5), Color.Black);
                }
                // Increment the y position
                position.Y += 20;
            }
        }
    }
}