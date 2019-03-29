using System.Collections.Generic;
using System.Linq;

namespace Fantomo.Core
{
    /// <summary>A room.</summary>
    sealed class Room : IRoom
    {
        /// <summary>Get address of this room.</summary>
        public Point Address { get; internal set; }

        /// <summary>Whether a trap exists in this room.</summary>
        public bool IsTrap { get; internal set; }

        /// <summary>Doors for access to neighbor rooms.</summary>
        public IReadOnlyDictionary<Direction, Door> Neighbors => InnerNeighbors;

        /// <summary>Doors for access to neighbor rooms.</summary>
        internal Dictionary<Direction, Door> InnerNeighbors { get; set; }

        public static IEnumerable<Point> CreateAddresses(Point size)
        {
            foreach (var y in Enumerable.Range(start: 0, count: size.Y)) {
                foreach (var x in Enumerable.Range(start: 0, count: size.X))
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}
