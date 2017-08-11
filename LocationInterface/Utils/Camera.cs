namespace LocationInterface.Utils
{
    public class Camera
    {
        public Vector2 Position { get; set; }

        public Camera()
        {
            Position = new Vector2(0, 0);
        }

        public void SetPos(double x, double y)
        {
            Position.X = x;
            Position.Y = y;
        }
        public void SetPos(Vector2 position)
        {
            Position = position;
        }

        public void Move(double x, double y)
        {
            Position.X += x;
            Position.Y += y;
        }
        public void Move(Vector2 position)
        {
            Position += position;
        }
    }
}
