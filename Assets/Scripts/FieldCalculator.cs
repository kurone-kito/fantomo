using UdonSharp;
using UnityEngine;

/// <summary>フィールド算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FieldCalculator : UdonSharpBehaviour
{
    /// <value>同期管理オブジェクト。</value>
    /// <value>管理ロジックの親となるオブジェクト。</value>
    [SerializeField]
    private GameObject managers;
    /// <value>定数一覧。</value>
    private Constants constants;
    /// <value>方角周りの計算ロジック。</value>
    private DirectionCalculator directionCalculator;
    /// <value>部屋情報算出のロジック。</value>
    private RoomsCalculator roomsCalculator;
    /// <value>同期管理オブジェクト。</value>
    private SyncManager syncManager;

    /// <summary>
    /// フィールドの計算をします。
    /// </summary>
    public byte[] Calculate()
    {
        if (this.constants == null)
        {
            Debug.LogError(
                "constants が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return null;
        }
        if (this.roomsCalculator == null)
        {
            Debug.LogError(
                "roomsCalculator が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return null;
        }
        return calculateIntenalRooms();
    }

    private byte[] calculateIntenalRooms()
    {
        var started = Time.realtimeSinceStartup;
        var NUM_KEYS = this.constants.NUM_KEYS;
        var NUM_PLAYERS = this.constants.NUM_PLAYERS;
        var ROOM_REMOVE_DOOR_RATE = this.constants.ROOM_REMOVE_DOOR_RATE;
        var ROOM_FLG_HAS_KEY = this.constants.ROOM_FLG_HAS_KEY;
        var ROOM_FLG_HAS_SPAWN = this.constants.ROOM_FLG_HAS_SPAWN;
        var rooms = roomsCalculator.CreateIdentityRooms();
        var cutted = this.cutRoute(rooms, ROOM_REMOVE_DOOR_RATE);
        var puttedMines = this.putMinesRecursively(cutted, 0);
        var puttedKeys =
            this.putItemsRecursively(
                puttedMines, NUM_KEYS, ROOM_FLG_HAS_KEY);
        var result = this.putItemsRecursively(
            puttedKeys, NUM_PLAYERS, ROOM_FLG_HAS_SPAWN);
        return result;
    }

    /// <summary>再帰的に指定数の扉を削除します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="removes">削除する扉の数。</param>
    /// <returns>新しい部屋情報一覧。</returns>
    [RecursiveMethod]
    private byte[] cutRouteRecursively(byte[] rooms, int removes)
    {
        if (removes < 0)
        {
            return rooms;
        }
        var dirs = this.directionCalculator.Direction;
        var targetIndex = Random.Range(0, rooms.Length);
        var dirMark = ~(uint)dirs[Random.Range(0, dirs.Length)];
        var nextRooms = new byte[rooms.Length];
        for (int i = rooms.Length; --i >= 0;)
        {
            nextRooms[i] =
                i == targetIndex ? (byte)(rooms[i] & dirMark) : rooms[i];
        }
        var explorable =
            this.roomsCalculator.GetExplorableRoomsLength(nextRooms);
        var next = explorable == rooms.Length;
        return this.cutRouteRecursively(
            next ? nextRooms : rooms, removes - (next ? 1 : 0));
    }

    /// <summary>扉を指定の確率で削除します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="percentage">確率を 0f～1f の間で指定します。</param>
    /// <returns>新しい部屋情報一覧。</returns>
    private byte[] cutRoute(byte[] rooms, float percentage) =>
        this.cutRouteRecursively(
            rooms,
            (int)(rooms.Length * this.constants.DIR_MAX * percentage));

    /// <summary>再帰的に地雷を設置します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="putted">設置済み地雷の個数。</param>
    /// <returns>新しい部屋情報一覧。</returns>
    [RecursiveMethod]
    private byte[] putMinesRecursively(byte[] rooms, int putted)
    {
        var NUM_MINES = this.constants.NUM_MINES;
        var ROOM_FLG_HAS_MINE = this.constants.ROOM_FLG_HAS_MINE;
        if (putted >= NUM_MINES)
        {
            return rooms;
        }
        var targetIndex = Random.Range(0, rooms.Length);
        if ((rooms[targetIndex] & ROOM_FLG_HAS_MINE) != 0)
        {
            return this.putMinesRecursively(rooms, putted);
        }
        var nextRooms = new byte[rooms.Length];
        for (int i = rooms.Length; --i >= 0;)
        {
            nextRooms[i] =
                i == targetIndex
                    ? (byte)(rooms[i] | ROOM_FLG_HAS_MINE)
                    : rooms[i];
        }
        var next =
            !this.roomsCalculator.HasUnExplorableMine(nextRooms) &&
            this.roomsCalculator.GetExplorableRoomsLength(nextRooms) ==
                rooms.Length - putted;
        return this.putMinesRecursively(
            next ? nextRooms : rooms, putted + (next ? 1 : 0));
    }

    /// <summary>再帰的にアイテムを設置します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="amount">設置個数。</param>
    /// <param name="itemFlag">アイテムを示すフラグ。</param>
    /// <returns>新しい部屋情報一覧。</returns>
    [RecursiveMethod]
    private byte[] putItemsRecursively(byte[] rooms, int amount, byte itemFlag)
    {
        if (amount <= 0)
        {
            return rooms;
        }
        var targetIndex = Random.Range(0, rooms.Length);
        if (this.roomsCalculator.HasAnyItems(rooms[targetIndex]))
        {
            return this.putItemsRecursively(rooms, amount, itemFlag);
        }
        var nextRooms = new byte[rooms.Length];
        for (int i = rooms.Length; --i >= 0;)
        {
            nextRooms[i] =
                i == targetIndex
                    ? (byte)(rooms[i] | itemFlag)
                    : rooms[i];
        }
        return this.putItemsRecursively(
            nextRooms, amount - 1, itemFlag);
    }

    /// <summary>
    /// <para>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </para>
    /// <para>ここでは、各フィールドの確保を行います。</para>
    /// </summary>
    void Start()
    {
        if (this.managers)
        {
            this.constants = this.managers.GetComponentInChildren<Constants>();
            this.directionCalculator = this.managers.GetComponentInChildren<DirectionCalculator>();
            this.roomsCalculator = this.managers.GetComponentInChildren<RoomsCalculator>();
            this.syncManager = this.managers.GetComponentInChildren<SyncManager>();
        }
        this.SendCustomEventDelayedSeconds(nameof(this.Calculate), .1f);
    }
}
