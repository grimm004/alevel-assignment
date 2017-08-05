using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        private const int BORDER = 25;
        private const double MRATE = .01;
        private const float ORATE = .5f;
        private int updateCounter = 0;
        private int pointSize = 2;
        private DatabaseManager.Database database;
        private List<Vector2> locations;
        private Vector2 currentPoint;
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
            database = new DatabaseManager.CSVDatabase("C:\\Data", false, ".csv");
            locations = new List<Vector2>();
            // Test Change
        }

        protected void UpdateMac()
        {
            Console.WriteLine("Enter new mac address:");
            string newMac = Console.ReadLine();
            DatabaseManager.Table table = database.GetTable("TUI_D1_location_data_03-12-2017");
            DatabaseManager.Record[] records = table.GetRecords("deck", "Deck4");
            Console.WriteLine(records.Length);
            locations = new List<Vector2>();
            for (int i = 0; i < records.Length; i++)
                if ((string)records[i].GetValue("mac") == newMac)
                    locations.Add(new Vector2((float)(double)records[i].GetValue("x"), (float)(double)records[i].GetValue("y")));
        }

        protected void RenderPointTexture(int size)
        {
            if (size < 0) return;
            pointTexture = new Texture2D(GraphicsDevice, size, size);
            Color[] data = new Color[size * size];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            pointTexture.SetData(data);
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
            leftBind = new Keybind(Keys.Left, () => RenderPointTexture(--pointSize), true);
            rightBind = new Keybind(Keys.Right, () => RenderPointTexture(++pointSize), true);
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
            RenderPointTexture(pointSize);
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
                if (currentPointIndex == locations.Count) currentPointIndex = 0;
                else currentPoint = locations[currentPointIndex++];
                updateCounter = 0;
            }

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
            if (!cyclePoints) foreach (Vector2 point in locations) spriteBatch.Draw(pointTexture, offset + ((float)multiplier * point), Color.White);
            else spriteBatch.Draw(pointTexture, offset + ((float)multiplier * currentPoint), Color.White);
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
}
