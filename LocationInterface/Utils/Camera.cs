using Microsoft.Xna.Framework;

namespace LocationInterface.Utils
{
    public class Camera
    {
        public Vector2 Position { get; private set; } = Vector2.Zero;
        public float Scale { get; set; } = 1f;

        public Matrix Transformation
        {
            get
            {
                // Fetch the translation matrix for the camera's position
                return Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position.X, Position.Y, 0);
            }
        }

        public void Move(int x, int y)
        {
            // Move the camera's position
            Position += new Vector2(x, y);
        }
    }
}
