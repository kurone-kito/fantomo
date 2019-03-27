using System.Collections.Generic;
using System.Linq;

namespace Fantomo.Core
{
    /// <summary>Extension for Direction enum.</summary>
    public static class DirectionExtension
    {
        /// <summary>Offsets of the direction.</summary>
        private static readonly IEnumerable<KeyValuePair<Direction, Point>> Offsets = new KeyValuePair<Direction, Point>[]
            {
                new KeyValuePair<Direction, Point>(Direction.Down, new Point(y: 1)),
                new KeyValuePair<Direction, Point>(Direction.Left, new Point(x: -1)),
                new KeyValuePair<Direction, Point>(Direction.Right, new Point(x: 1)),
                new KeyValuePair<Direction, Point>(Direction.Up, new Point(y: -1)),
            };

        /// <summary>Get offset of the direction.</summary>
        /// <param name="direction">direction.</param>
        public static Point Offset(this Direction direction)
        {
            var filtered =
                from o in Offsets
                where (o.Key & direction) != 0
                select o.Value;
            return filtered.Aggregate((a, b) => a + b);
        }
    }
}
