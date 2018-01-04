using Microsoft.Xna.Framework;
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

        protected override void Initialize()
        {
            GraphicsDeviceManager = new WpfGraphicsDeviceService(this);

            WpfKeyboard = new WpfKeyboard(this);
            WpfMouse = new WpfMouse(this);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera();

            SelectedImageFile = new ImageFile();
            
            base.Initialize();

            int radius = 5, diameter = radius * 2;
            
            PointTexture = new Texture2D(GraphicsDevice, diameter, diameter);
            Color[] circleData = new Color[diameter * diameter];
            int i = 0;
            for (int x = -radius; x < radius; x++)
                for (int y = -radius; y < radius; y++)
                    if ((x * x) + (y * y) < radius * radius) circleData[i++] = Color.White;
                    else circleData[i++] = new Color(0, 0, 0, 0);
            PointTexture.SetData(circleData);
            TimeBased = false;

            Random = new Random();
            MacPointCollections = new MacPointCollection[0];

            Font = Content.Load<SpriteFont>("Font");

            SKeyBind = new KeyListener(Keys.S, SaveInfo);
        }

        public void LoadPoints(MacPointCollection[] points)
        {
            MacPointCollections = points;
        }

        public void LoadMap(ImageFile selectedImageFile)
        {
            SelectedImageFile = selectedImageFile;
            FileStream fileStream = new FileStream($"{ SettingsManager.Active.ImageFolder }\\{ selectedImageFile.FileName }", FileMode.Open);
            MapTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
            fileStream.Dispose();
        }

        protected void SaveInfo()
        {
            if (WpfKeyboard.GetState().IsKeyDown(Keys.LeftControl))
                App.ImageIndex.SaveIndex();
        }
        
        protected override void Update(GameTime time)
        {
            var mouseState = WpfMouse.GetState();
            var keyboardState = WpfKeyboard.GetState();

            SKeyBind.Update(keyboardState);
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

        public void ScalePoints(float change)
        {
            SelectedImageFile.Multiplier += change;
        }

        public void TranslatePoints(float x, float y)
        {
            SelectedImageFile.Offset += new Vector2((float)x, (float)y);
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(Color.White);

            SpriteBatch.Begin(transformMatrix: Camera.Transformation);
            DrawPoints();
            SpriteBatch.End();

            SpriteBatch.Begin();
            DrawKey();
            SpriteBatch.End();
        }

        protected void DrawPoints()
        {
            if (MapTexture != null) SpriteBatch.Draw(MapTexture, Vector2.Zero);
            foreach (MacPointCollection macPointCollection in MacPointCollections)
                foreach (Vector2 macPoint in macPointCollection.MacPoints)
                    SpriteBatch.Draw(PointTexture, SelectedImageFile.Offset + (SelectedImageFile.Multiplier * macPoint), null, macPointCollection.Colour, 0f, new Vector2(5), 1f, SpriteEffects.None, 0);
        }

        protected void DrawKey()
        {
            Vector2 position = new Vector2(20, 20);
            for (int i = 0; i < MacPointCollections.Length; i++)
            {
                SpriteBatch.Draw(PointTexture, position, null, MacPointCollections[i].Colour, 0f, new Vector2(5), 1f, SpriteEffects.None, 0);
                SpriteBatch.DrawString(Font, MacPointCollections[i].Address, position + new Vector2(10, -5), Color.Black);
                if (TimeBased)
                {
                    SpriteBatch.DrawString(Font, MacPointCollections[i].MacPoints.Count > 0 ? MacPointCollections[i].MacPoints[0].Time.ToString(@"hh\:mm\:ss") : "No Points", position + new Vector2(125, -5), Color.Black);
                    if (MacPointCollections[i].MacPoints.Count > 0) SpriteBatch.DrawString(Font, MacPointCollections[i].MacPoints[0].Node, position + new Vector2(180, -5), Color.Black);
                }
                position.Y += 20;
            }
        }
    }
}
