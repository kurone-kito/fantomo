using System;

namespace Fantomo.Core
{
    /// <summary>Doors direction.</summary>
    [Flags]
    public enum Direction
    {
        /// <summary>Down (south)</summary>
        Down = 0x1,

        /// <summary>Left (west)</summary>
        Left = 0x2,

        /// <summary>Right (east)</summary>
        Right = 0x4,

        /// <summary>Up (north)</summary>
        Up = 0x8,
    }
}
