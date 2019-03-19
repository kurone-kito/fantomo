using System;
using System.Collections.Generic;
using System.Linq;

namespace Fantomo.Core
{
    public sealed class Rooms
    {
        public Point size { get; private set; }
        // private static readonly (Direction type, int x, int y)[] NeighborTable =
        //     new[] {
        //         (type: Direction.Right, x: 1, y: 0),
        //         (type: Direction.Down, x: 0, y: 1),
        //         (type: Direction.Left, x: -1, y: 0),
        //         (type: Direction.Up, x: 0, y: -1),
        //         (type: Direction.Right | Direction.Up, x: 1, y: -1),
        //         (type: Direction.Right | Direction.Down, x: 1, y: 1),
        //         (type: Direction.Left | Direction.Up, x: -1, y: -1),
        //         (type: Direction.Left | Direction.Down, x: -1, y: 1),
        //     };

        // private static IEnumerable<(Direction type, int x, int y)> GetNeighborIndex((int x, int y) target, (int width, int height) limit)
        // {
        //     foreach (var neighbor in NeighborTable)
        //     {
        //         var x = target.x + neighbor.x;
        //         var y = target.x + neighbor.y;
        //         if (x >= 0 && y >= 0 && x < limit.width && y < limit.height)
        //         {
        //             yield return (neighbor.type, x, y);
        //         }
        //     }
        // }

        // private static void InitializeNeighbors(Room[][] map)
        // {
        //     var width = map.Length;
        //     for (var y = width; --y >= 0;)
        //     {
        //         var row = map[y];
        //         var height = row.Length;
        //         for (var x = height; --x >= 0;)
        //         {
        //             var offsets = GetNeighborIndex((x, y), (width, height));
        //             var room = row[x];
        //             room.InnerNeighbors = offsets.ToDictionary(o => o.type, o => new Door());
        //         }
        //     }
        // }

        // private readonly Room[][] InnerMap;

        // public IReadOnlyCollection<IReadOnlyCollection<IRoom>> Map
        // {
        //     get { return InnerMap; }
        // }

        // public Rooms(int width, int height)
        // {
        //     var createRows = Enumerable.Repeat<Func<Room[]>>(() => new Room[width], height);
        //     var innerMap = (from createRow in createRows select createRow()).ToArray();
        //     InitializeNeighbors(innerMap);
        //     InnerMap = innerMap;
        // }
    }
}
