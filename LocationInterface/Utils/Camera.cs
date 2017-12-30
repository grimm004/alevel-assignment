using Microsoft.Xna.Framework;

namespace LocationInterface.Utils
{
    public class Camera
    {
        public Vector2 Position { get; protected set; }

        public Matrix Transformation
        {
            get
            {
                return Matrix.CreateTranslation(
                    Position.X,
                    Position.Y, 0);
            }
        }

        public void Move(int x, int y)
        {
            Position += new Vector2(x, y);
        }
    }
}
