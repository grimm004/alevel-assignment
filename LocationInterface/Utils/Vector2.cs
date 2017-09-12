using Newtonsoft.Json;

namespace LocationInterface.Utils
{
    public class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Initialise the Vector (Zero vector by default)
        /// </summary>
        public Vector2() { X = Y = 0; }
        /// <summary>
        /// Initialise the Vector
        /// </summary>
        /// <param name="value">The initial value for X and Y</param>
        public Vector2(double value) { X = Y = value; }
        /// <summary>
        /// Initialise the Vector
        /// </summary>
        /// <param name="x">The initial X value</param>
        /// <param name="y">The initial Y value</param>
        public Vector2(double x, double y) { X = x; Y = y; }

        [JsonIgnore]
        public Vector2 Copy { get { return new Vector2(X, Y); } }

        /// <summary>
        /// Multiply the vector by a scalar
        /// </summary>
        /// <param name="left">The scalar</param>
        /// <param name="right">The vector</param>
        /// <returns>the scaled vector</returns>
        public static Vector2 operator *(double left, Vector2 right)
        {
            return new Vector2(left * right.X, left * right.Y);
        }
        /// <summary>
        /// Adds two vectors together
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>the sum vector of the two vectors</returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        [JsonIgnore]
        public static Vector2 Zero
        {
            get { return new Vector2(); }
        }
        [JsonIgnore]
        public static Vector2 Unit
        {
            get { return new Vector2(1); }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString()
        {
            return $"Vector2({ X.ToString("0.00") }, { Y.ToString("0.00") })";
        }
    }
}
