using AnalysisSDK;
using DatabaseManagerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MapTestPlugin
{
    public class MapTest : IMapper
    {
        public string Name { get { return "Map Test"; } }
        public string Description { get { return "Map Test"; } }

        public void Initialize(ContentManager content)
        {

        }

        public void LoadTables(Table[] tables)
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(StandardContent.Font, "Map Test", new Vector2(250), Color.Black);
            spriteBatch.End();
        }
    }
}
