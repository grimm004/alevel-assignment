using System.Windows.Controls;
using System.Windows.Shapes;

namespace LocationInterface.Utils
{
    public class Point
    {
        protected Vector2 OriginalPosition { get; set; }
        protected Vector2 Position { get; set; }
        protected bool Initialised { get; set; }
        public Ellipse Ellipse { get; protected set; }

        public Point()
        {
            OriginalPosition = Vector2.Zero;
            Position = Vector2.Zero;
        }
        public Point(Vector2 position)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
        }
        public Point(double x, double y)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
        }
        public Point(Vector2 position, Ellipse ellipse)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }
        public Point(double x, double y, Ellipse ellipse)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }

        public void SetEllipse(Ellipse ellipse)
        {
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
            Initialised = true;
        }

        public void Update(Camera camera, Vector2 offset, double multiplier)
        {
            Position = (multiplier * OriginalPosition) + camera.Position + offset;
            if (Initialised)
            {
                Canvas.SetLeft(Ellipse, Position.X);
                Canvas.SetTop(Ellipse, Position.Y);
            }
        }
    }
}
