namespace LocationInterface.Utils
{
    public class Camera
    {
        public Vector2 Position { get; protected set; }

        /// <summary>
        /// Initialise the camera object
        /// </summary>
        public Camera()
        {
            // Define the default camera position of (0, 0)
            Position = new Vector2(0, 0);
        }

        /// <summary>
        /// Set the position of the camera
        /// </summary>
        /// <param name="x">x position of the camera</param>
        /// <param name="y">y position of the camera</param>
        public void SetPos(double x, double y)
        {
            // Set the position of the camera to a new vector with the desired position
            Position = new Vector2(x, y);
        }
        /// <summary>
        /// Set the position of the camera
        /// </summary>
        /// <param name="position">The new position vector for the camera</param>
        public void SetPos(Vector2 position)
        {
            // Set the position of the camera to the desired position vector
            Position = position;
        }

        /// <summary>
        /// Move the camera
        /// </summary>
        /// <param name="x">The magnitude of offset in the x direction</param>
        /// <param name="y">The magnitude of offset in the y direction</param>
        public void Move(double x, double y)
        {
            // Add a new vector of the desired x and y change to the position
            Position += new Vector2(x, y);
        }
        /// <summary>
        /// Move the camera
        /// </summary>
        /// <param name="change">The offset vector to move the camera with</param>
        public void Move(Vector2 change)
        {
            // Add the desired change to the position
            Position += change;
        }
    }
}
