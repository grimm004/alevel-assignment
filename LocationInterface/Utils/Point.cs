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

        /// <summary>
        /// Initialise a Point
        /// </summary>
        public Point()
        {
            OriginalPosition = Vector2.Zero;
            Position = Vector2.Zero;
        }
        /// <summary>
        /// Initialise a Point
        /// </summary>
        /// <param name="position">The initial postion of the point</param>
        public Point(Vector2 position)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
        }
        /// <summary>
        /// Initialise a Point
        /// </summary>
        /// <param name="x">The initial x position of the point</param>
        /// <param name="y">The initial y position of the point</param>
        public Point(double x, double y)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
        }
        /// <summary>
        /// Initialise a Point
        /// </summary>
        /// <param name="position">The initial position of the point</param>
        /// <param name="ellipse">The canvas ellipse to render as the point</param>
        public Point(Vector2 position, Ellipse ellipse)
        {
            OriginalPosition = position.Copy;
            Position = position.Copy;
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }
        /// <summary>
        /// Initialise a Point
        /// </summary>
        /// <param name="x">The initial x position of the point</param>
        /// <param name="y">The initial y position of the point</param>
        /// <param name="ellipse">The canvas ellipse to redner as the point</param>
        public Point(double x, double y, Ellipse ellipse)
        {
            OriginalPosition = new Vector2(x, y);
            Position = new Vector2(x, y);
            Ellipse = ellipse;
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
        }

        /// <summary>
        /// Set the canvas ellipse to render as the point
        /// </summary>
        /// <param name="ellipse">The canvas ellipse instance to render</param>
        public void SetEllipse(Ellipse ellipse)
        {
            // Set the ellipse
            Ellipse = ellipse;
            // Set the X and Y postion of the ellipse
            Canvas.SetLeft(Ellipse, Position.X);
            Canvas.SetTop(Ellipse, Position.Y);
            // Mark as initialised
            Initialised = true;
        }

        /// <summary>
        /// Update the point
        /// </summary>
        /// <param name="camera">The camera instance</param>
        /// <param name="offset">The point offset</param>
        /// <param name="multiplier">The point multiplier ratio</param>
        public void Update(Camera camera, Vector2 offset, double multiplier)
        {
            // Calculate the absolute position taking into account its position, the camera position and the general offset/multiplier.
            Position = (multiplier * OriginalPosition) + camera.Position + offset;
            // If the ellipse is initialised
            if (Initialised)
            {
                // Set the X and Y position of the point
                Canvas.SetLeft(Ellipse, Position.X);
                Canvas.SetTop(Ellipse, Position.Y);
            }
        }
    }
}
