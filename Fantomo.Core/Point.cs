using System.Collections.Generic;

namespace Fantomo.Core
{
    public struct Point
    {

        public int X { get; }
        public int Y { get; }
        public Point(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
    }
}
