using UdonSharp;
using UnityEngine;

/// <summary>部屋情報算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class RoomsCalculator : UdonSharpBehaviour
{
    /// <summary>無効を示す、隣接部屋のインデックス。</summary>
    private const byte INVALID_NEIGHBOR_INDEX = 0xFF;

    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;
    /// <summary>定数一覧。</summary>
    private Constants constants;
    /// <summary>方角周りの計算ロジック。</summary>
    private DirectionCalculator directionCalculator;
    /// <summary>探索済みの部屋インデックス一覧。</summary>
    private int[] visited;

    /// <summary>初期状態の部屋情報一覧を取得します。</summary>
    public byte[] CreateIdentityRooms()
    {
        if (constants == null)
        {
            Debug.LogError(
                "constants が null のため、部屋を算出できません。: RoomsCalculator.CreateIdentityRooms");
            return null;
        }
        var NUM_ROOMS = constants.NUM_ROOMS;
        var rooms = new byte[NUM_ROOMS];
        for (var i = NUM_ROOMS; --i >= 0;)
        {
            rooms[i] = (byte)ROOM_FLG.DIR_ALL;
        }
        return rooms;
    }

    /// <summary>
    /// 探索可能な部屋数を取得するために、再帰的探索を行います。
    /// </summary>
    /// <param name="rooms">現在の部屋状況一覧。</param>
    /// <returns>探索可能な部屋数。</returns>
    public int GetReachableRoomsLength(byte[] rooms)
    {
        if (constants == null)
        {
            Debug.LogError(
                "constants が null のため、部屋を算出できません。: RoomsCalculator.GetReachableRoomsLength");
            return 0;
        }
        for (var i = rooms.Length; --i >= 0;)
        {
            visited[i] = -1;
        }
        return getReachableRoomsLengthRecursive(rooms, 0) + 1;
    }

    /// <summary>
    /// 部屋が任意のアイテムフラグを持っているかどうかを取得します。
    /// </summary>
    /// <param name="room">部屋のビット情報。</param>
    /// <returns>任意のアイテムフラグを持っている場合、<c>true</c></returns>
    public bool HasAnyItems(byte room)
    {
        if (constants == null)
        {
            Debug.LogError(
                "constants が null のため、部屋を算出できません。: RoomsCalculator.HasAnyItems");
            return false;
        }
        return (room & (byte)ROOM_FLG.HAS_ALL) != 0;
    }

    /// <summary>
    /// 探索不能な地雷が存在するかどうかを取得します。
    /// </summary>
    /// <param name="rooms">現在の部屋状況一覧。</param>
    /// <returns>探索不能な地雷が存在する場合、<c>true</c>。</returns>
    public bool HasUnReachableMine(byte[] rooms)
    {
        if (constants == null)
        {
            Debug.LogError(
                "constants が null のため、部屋を算出できません。: RoomsCalculator.HasUnReachableMine");
            return false;
        }
        for (var i = rooms.Length; --i >= 0;)
        {
            var room = rooms[i];
            if ((room & (byte)ROOM_FLG.HAS_MINE) == 0)
            {
                continue;
            }
            var neighbors = getNeighborIndexes(room, i);
            var founds = 0;
            for (int j = (int)DIR.MAX; --j >= 0;)
            {
                var ni = (neighbors >> (j * 8)) & 0xFF;
                if (ni != INVALID_NEIGHBOR_INDEX &&
                    (rooms[ni] & (byte)ROOM_FLG.HAS_MINE) != 0)
                {
                    founds++;
                }
            }
            if (founds >= (int)DIR.MAX)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>通じていないルートの削除をします。</summary>
    /// <param name="rooms">現在の部屋状況一覧。</param>
    /// <returns>新しい部屋状況一覧。。</returns>
    private byte[] closeInvalidDirections(byte[] rooms)
    {
        var directions = directionCalculator.Direction;
        var invertDirections = directionCalculator.InvertDirection;
        var result = new byte[rooms.Length];
        for (int i = rooms.Length; --i >= 0;)
        {
            var room = rooms[i];
            var neighbors = getNeighborIndexes(room, i);
            for (int j = (int)DIR.MAX; --j >= 0;)
            {
                var ni = (neighbors >> (j * 8)) & 0xFF;
                var closed =
                    ni != INVALID_NEIGHBOR_INDEX &&
                    (rooms[ni] & invertDirections[j]) == 0;
                result[i] = closed ? (byte)(room | directions[j]) : room;
            }
        }
        return result;
    }

    /// <summary>
    /// 探索可能な部屋数を取得するために、再帰的探索を行います。
    /// </summary>
    /// <param name="rooms">現在の部屋状況一覧。</param>
    /// <param name="index">現在探索中の部屋インデックス。</param>
    /// <returns>探索可能な部屋数。</returns>
    [RecursiveMethod]
    private int getReachableRoomsLengthRecursive(byte[] rooms, int index)
    {
        var visited = this.visited;
        var neighbors = getNeighborIndexes(rooms[index], index);
        for (var i = visited.Length; --i >= 0;)
        {
            if (visited[i] < 0)
            {
                visited[i] = index;
                break;
            }
        }
        var result = 0;
        for (int i = (int)DIR.MAX; --i >= 0;)
        {
            var ni = (neighbors >> (i * 8)) & 0xFF;
            if (
                ni == INVALID_NEIGHBOR_INDEX ||
                (rooms[ni] & (byte)ROOM_FLG.HAS_MINE) != 0)
            {
                continue;
            }
            var found = false;
            for (int j = visited.Length; --j >= 0;)
            {
                if (visited[j] == ni)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                continue;
            }
            result +=
                1 + getReachableRoomsLengthRecursive(rooms, (int)ni);
        }
        return result;
    }

    /// <summary>インデックスから座標を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>上位 4bit にX座標、下位 4bit にY座標。</returns>
    private byte getXYFromIndex(int index)
    {
        var width = constants.ROOMS_WIDTH;
        var x = (index % width) & 0xF;
        var y = (index / width) & 0xF;
        return (byte)((x << 4) + y);
    }

    /// <summary>座標からインデックスを取得します。</summary>
    /// <param name="x">X 座標。</param>
    /// <param name="y">Y 座標。</param>
    /// <returns>インデックス。はみ出た場合、255。</returns>
    private byte getIndexFromXY(int x, int y)
    {
        var width = constants.ROOMS_WIDTH;
        return x < 0 || x >= width || y < 0 || y >= width
            ? (byte)0xFF
            : (byte)((y * width) + x);
    }

    /// <summary>隣接する部屋のインデックス一覧を取得します。</summary>
    /// <param name="room">部屋のビット情報。</param>
    /// <param name="index">インデックス。</param>
    /// <returns>
    /// 隣接する部屋のインデックス一覧を、上位から 8bit ごとに
    /// +X、-X、+Y、-Y の順で返します。通路がふさがっている場合は、255。
    /// </returns>
    private uint getNeighborIndexes(byte room, int index)
    {
        var INV = INVALID_NEIGHBOR_INDEX;
        var constants = this.constants;
        if (constants == null)
        {
            return packNeighborIndexes(INV, INV, INV, INV);
        }
        var xy = getXYFromIndex(index);
        var x = xy & 0xF;
        var y = (xy >> 4) & 0xF;
        return packNeighborIndexes(
            (room & (1 << (byte)ROOM_FLG.DIR_N)) == 0
                ? INV
                : getIndexFromXY(x - 1, y),
            (room & (1 << (byte)ROOM_FLG.DIR_S)) == 0
                ? INV
                : getIndexFromXY(x + 1, y),
            (room & (1 << (byte)ROOM_FLG.DIR_W)) == 0
                ? INV
                : getIndexFromXY(x, y - 1),
            (room & (1 << (byte)ROOM_FLG.DIR_E)) == 0
                ? INV
                : getIndexFromXY(x, y + 1)
        );
    }

    /// <summary>隣接する部屋のインデックス一覧を取得します。</summary>
    /// <param name="n">-Y 位置のインデックス。</param>
    /// <param name="s">+Y 位置のインデックス。</param>
    /// <param name="w">-X 位置のインデックス。</param>
    /// <param name="e">+X 位置のインデックス。</param>
    /// <returns>
    /// 隣接する部屋のインデックス一覧を、上位から 8bit ごとに
    /// +X、-X、+Y、-Y の順で返します。
    /// </returns>
    private uint packNeighborIndexes(byte n, byte s, byte w, byte e)
    {
        return ((uint)e << 24) | ((uint)w << 16) | ((uint)s << 8) | n;
    }

    /// <summary>
    /// <para>このコンポーネントが初期化された時に呼び出す、コールバック。</para>
    /// <para>ここでは、各フィールドの確保を行います。</para>
    /// </summary>
    private void Start()
    {
        if (managers)
        {
            constants =
                managers.GetComponentInChildren<Constants>();
            directionCalculator =
                managers.GetComponentInChildren<DirectionCalculator>();
            visited = new int[constants.NUM_ROOMS];
        }
    }
}
