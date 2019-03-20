using System.Collections.Generic;

namespace Fantomo.Core
{
    public static class OffsetHelper
    {
        private static KeyValuePair<Direction, Point>[] Offsets =
            {
                new KeyValuePair<Direction, Point>(Direction.Down, new Point(y: 1)),
                new KeyValuePair<Direction, Point>(Direction.Left, new Point(x: -1)),
                new KeyValuePair<Direction, Point>(Direction.Right, new Point(x: 1)),
                new KeyValuePair<Direction, Point>(Direction.Up, new Point(y: -1)),
            };

        public static Point Offset(Direction direction)
        {

        }
    }
}
