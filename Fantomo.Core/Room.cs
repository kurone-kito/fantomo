using System.Collections.Generic;

namespace Fantomo.Core
{
    sealed class Room : IRoom
    {
        public bool IsTrap { get; private set; }
        public IReadOnlyDictionary<Direction, Door> Neighbors
        {
            get { return InnerNeighbors; }
        }
        public Dictionary<Direction, Door> InnerNeighbors
        {
            get;
            set;
        }
    }
}