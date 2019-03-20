using System.Collections.Generic;

namespace Fantomo.Core
{
    /// <summary>Interface of a room.</summary>
    public interface IRoom
    {
        Point Address { get; }

        /// <summary>Whether a trap exists in this room.</summary>
        bool IsTrap { get; }

        /// <summary>Doors for access to neighbor rooms.</summary>
        IReadOnlyDictionary<Direction, Door> Neighbors { get; }
    }
}
