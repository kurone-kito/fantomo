namespace Fantomo.Core
{
    /// <summary>2D Position.</summary>
    public struct Point
    {
        /// <summary>X axis position.</summary>
        public int X { get; }

        /// <summary>Y axis position.</summary>
        public int Y { get; }

        /// <summary>Initialize the instance.</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public Point GetNeighbor(Direction direction)
        {
            return this + direction.Offset();
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
    }
}
