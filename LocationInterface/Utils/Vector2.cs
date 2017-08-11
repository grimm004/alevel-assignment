namespace LocationInterface.Utils
{
    public class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2() { X = Y = 0; }
        public Vector2(double value) { X = Y = value; }
        public Vector2(double x, double y) { X = x; Y = y; }

        public Vector2 Copy { get { return new Vector2(X, Y); } }

        public static Vector2 operator *(double left, Vector2 right)
        {
            return new Vector2(left * right.X, left * right.Y);
        }
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        public static Vector2 Zero
        {
            get { return new Vector2(); }
        }
    }
}
