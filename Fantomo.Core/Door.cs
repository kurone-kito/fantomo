namespace Fantomo.Core
{
    public struct Door
    {
        public IRoom[] Rooms { get; }
        public bool IsLocked { get; }

        public Door(IRoom[] rooms, bool locked = false)
        {
            Rooms = rooms;
            IsLocked = locked;
        }


        public Door Toggle()
        {
            return new Door(rooms: Rooms, locked: !IsLocked);
        }
    }
}
