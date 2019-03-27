namespace Fantomo.Core
{
    /// <summary>A door.</summary>
    public sealed class Door
    {
        /// <summary>Whether this door is locked.</summary>
        public bool IsLocked { get; internal set; } = false;

        /// <summary>Create a new instance with the locked state switched.</summary>
        public Door Toggle()
        {
            return new Door() { IsLocked = !IsLocked };
        }
    }
}
