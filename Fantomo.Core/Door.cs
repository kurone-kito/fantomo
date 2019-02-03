namespace Fantomo.Core
{
    public struct Door
    {
        public IRoom Room { get; }
        public bool IsLocked { get; }

        public Door(IRoom room, bool locked = false)
        {
            Room = room;
            IsLocked = locked;
        }

        public Door Toggle()
        {
            return new Door(room: Room, locked: !IsLocked);
        }
    }
}