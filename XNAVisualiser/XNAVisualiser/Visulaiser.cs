using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNAVisualiser
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Visulaiser : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D currentDeck;
        private Texture2D pointTexture;
        private Texture2D pointTextureBlue;
        private const int BORDER = 25;
        private const double MRATE = .01;
        private const float ORATE = .5f;
        private int updateCounter = 0;
        private int pointSize = 2;
        private DatabaseManagerLibrary.Database database;
        private List<Point> points;
        private Point currentPoint;
        private int currentPointIndex = 0;
        private double multiplier = 7;
        private bool cyclePoints = false;
        private Vector2 offset = new Vector2(BORDER);
        private Keybind upBind;
        private Keybind downBind;
        private Keybind leftBind;
        private Keybind rightBind;
        private Keybind wBind;
        private Keybind aBind;
        private Keybind sBind;
        private Keybind dBind;
        private Keybind tBind;
        private Keybind yBind;
        private Keybind[] binds;

        public Visulaiser()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            database = new DatabaseManagerLibrary.CSV("C:\\Data", false, ".csv");
            points = new List<Point>();
            Console.WriteLine(database.GetTable("LocationCount"));
            foreach (DatabaseManagerLibrary.Record record in database.GetTable("LocationCount").GetRecords()) Console.WriteLine(record);
        }

        protected void UpdateMac()
        {
            Console.WriteLine("Enter new mac address:");
            string newMac = Console.ReadLine();
            DatabaseManagerLibrary.Table table = database.GetTable("TUI_D1_location_data_03-12-2017");
            Console.WriteLine("Loading...");
            DatabaseManagerLibrary.Record[] records = table.GetRecords("mac", newMac);
            points = new List<Point>();
            for (int i = 0; i < records.Length; i++)
                if ((string)records[i].GetValue("deck") == "Deck4") if ((string)records[i].GetValue("locationid") == "POO000447DEGSB") points.Add(new Point((float)(double)records[i].GetValue("x"), (float)(double)records[i].GetValue("y"), pointTextureBlue));
                    else points.Add(new Point((float)(double)records[i].GetValue("x"), (float)(double)records[i].GetValue("y"), pointTexture));
            Console.WriteLine("Done!");
        }

        protected void RenderPointTextures(int size)
        {
            if (size < 0) return;
            pointTexture = new Texture2D(GraphicsDevice, size, size);
            pointTextureBlue = new Texture2D(GraphicsDevice, size, size);

            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            pointTexture.SetData(data);
            
            for (int i = 0; i < data.Length; i++) data[i] = Color.Blue;
            pointTextureBlue.SetData(data);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            upBind = new Keybind(Keys.Up, () => multiplier += MRATE, false);
            downBind = new Keybind(Keys.Down, () => multiplier -= MRATE, false);
            leftBind = new Keybind(Keys.Left, () => RenderPointTextures(--pointSize), true);
            rightBind = new Keybind(Keys.Right, () => RenderPointTextures(++pointSize), true);
            wBind = new Keybind(Keys.W, () => offset.Y -= ORATE, false);
            aBind = new Keybind(Keys.A, () => offset.X -= ORATE, false);
            sBind = new Keybind(Keys.S, () => offset.Y += ORATE, false);
            dBind = new Keybind(Keys.D, () => offset.X += ORATE, false);
            tBind = new Keybind(Keys.T, UpdateMac, true);
            yBind = new Keybind(Keys.Y, () => cyclePoints ^= true, true);
            binds = new Keybind[] { upBind, downBind, leftBind, rightBind, wBind, aBind, sBind, dBind, tBind, yBind };
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            RenderPointTextures(pointSize);
            currentDeck = Content.Load<Texture2D>("Deck4");
            graphics.PreferredBackBufferWidth = currentDeck.Bounds.Width + (2 * BORDER);
            graphics.PreferredBackBufferHeight = currentDeck.Bounds.Height + (2 * BORDER);
            graphics.ApplyChanges();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();
            foreach (Keybind bind in binds) bind.Update();
            
            if (++updateCounter % 30 == 0)
            {
                if (currentPointIndex == points.Count) currentPointIndex = 0;
                else currentPoint = points[currentPointIndex++];
                updateCounter = 0;
            }

            foreach (Point point in points) point.Update(offset, multiplier);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(currentDeck, new Vector2(BORDER), Color.White);
            if (!cyclePoints) foreach (Point point in points) point.Draw(spriteBatch);
            else currentPoint.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    class Keybind
    {
        public Keys Key { get; set; }
        public Action Command { get; set; }
        public bool LockKey { get; set; }
        private bool keyLock;

        public Keybind(Keys key, Action command, bool lockKey)
        {
            Key = key;
            Command = command;
            LockKey = lockKey;
        }

        public void Update()
        {
            bool keyDown = Keyboard.GetState().IsKeyDown(Key);
            if (keyDown && (!keyLock || !LockKey)) { Command?.Invoke(); keyLock = true; }
            else if (!keyDown && keyLock) keyLock = false;
        }
    }

    class Point
    {
        public Vector2 OriginalPosition { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public Point() { }
        public Point(Vector2 position, Texture2D texture)
        {
            OriginalPosition = new Vector2(position.X, position.Y);
            Position = new Vector2(position.X, position.Y);
            Texture = texture;
        }

        public Point(float x, float y, Texture2D texture)
        {
            OriginalPosition = Position = new Vector2(x, y);
            Texture = texture;
        }

        public void Update(Vector2 offset, double multiplier)
        {
            Position = (float)multiplier * (offset + OriginalPosition);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, Color.White);
        }
    }
}