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
        private const int POINTSIZE = 2;
        private const double MRATE = .01;
        private const float ORATE = .5f;
        private DatabaseManager.Database database;
        private Vector2[] locations;
        private double multiplier = 7;
        private Vector2 offset = new Vector2(BORDER);
        private Keybind incrBind;
        private Keybind decrBind;
        private Keybind wBind;
        private Keybind aBind;
        private Keybind sBind;
        private Keybind dBind;
        private Keybind[] binds;

        public Visulaiser()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            database = new DatabaseManager.CSVDatabase("C:\\Data", false, ".csv");
            DatabaseManager.Table table = database.GetTable("TUI_D1_location_data_03-12-2017");
            DatabaseManager.Record[] records3 = table.GetRecords("deck", "Deck3");
            DatabaseManager.Record[] records4 = table.GetRecords("deck", "Deck4");
            Console.WriteLine(records4.Length);
            locations = new Vector2[records4.Length];
            for (int i = 0; i < locations.Length; i++) locations[i] = new Vector2((float)(double)records4[i].GetValue("x"), (float)(double)records4[i].GetValue("y"));
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
            incrBind = new Keybind(Keys.Up, () => { multiplier += MRATE; }, false);
            decrBind = new Keybind(Keys.Down, () => { multiplier -= MRATE; }, false);
            wBind = new Keybind(Keys.W, () => { offset.Y -= ORATE; }, false);
            aBind = new Keybind(Keys.A, () => { offset.X -= ORATE; }, false);
            sBind = new Keybind(Keys.S, () => { offset.Y += ORATE; }, false);
            dBind = new Keybind(Keys.D, () => { offset.X += ORATE; }, false);
            binds = new Keybind[] { incrBind, decrBind, wBind, aBind, sBind, dBind };
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pointTexture = new Texture2D(GraphicsDevice, POINTSIZE, POINTSIZE);
            Color[] data = new Color[POINTSIZE * POINTSIZE];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
            pointTexture.SetData(data);
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
            foreach (Vector2 point in locations) spriteBatch.Draw(pointTexture, offset + ((float)multiplier * point), Color.White);
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
