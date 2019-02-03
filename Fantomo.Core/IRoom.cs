using System.Collections.Generic;

namespace Fantomo.Core
{
    public interface IRoom
    {
        bool IsTrap { get; }
        IReadOnlyDictionary<Direction, Door> Neighbors { get; }
    }
}