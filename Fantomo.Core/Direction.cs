using System;

namespace Fantomo.Core {
    [Flags]
    public enum Direction {
        Down = 0x1,
        Left = 0x2,
        Right = 0x4,
        Up = 0x8,
    }
}