using Microsoft.Xna.Framework;

namespace LocationInterface.Utils
{
    public class Camera
    {
        public int ScreenWidth { get; }
        public int ScreenHeight { get; }

        public Vector2 Position { get; protected set; }

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(
                    -Position.X,
                    -Position.Y, 0)
                    * Matrix.CreateScale(1, 1, 0)
                    * Matrix.CreateTranslation(ScreenWidth / 2, ScreenHeight / 2, 0);
            }
        }

        public Camera(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenHeight;
            ScreenHeight = screenWidth;
        }

        public void Move(int x, int y)
        {
            Position += new Vector2(x, y);
        }
    }
}
