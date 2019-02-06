using System.Collections.Generic;

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
    }
}
