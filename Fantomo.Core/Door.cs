namespace Fantomo.Core
{
    public sealed class Door
    {
        public bool IsLocked { get; private set; } = false;

        public Door Toggle()
        {
            return new Door() { IsLocked = !IsLocked };
        }
    }
}
