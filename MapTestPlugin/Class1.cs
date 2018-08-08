using AnalysisSDK;
using DatabaseManagerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTestPlugin
{
    public class MapTest : IMapper
    {
        public string Name { get { return "Map Test"; } }

        public string Description { get { return "Map Test"; } }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        public void LoadTables(Table[] tables)
        {

        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
