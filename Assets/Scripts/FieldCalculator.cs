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
        var NUM_KEYS = this.constants.NUM_KEYS;
        var NUM_PLAYERS = this.constants.NUM_PLAYERS;
        var ROOM_REMOVE_DOOR_RATE = this.constants.ROOM_REMOVE_DOOR_RATE;
        var ROOM_FLG_HAS_KEY = this.constants.ROOM_FLG_HAS_KEY;
        var ROOM_FLG_HAS_SPAWN = this.constants.ROOM_FLG_HAS_SPAWN;
        var rooms = roomsCalculator.CreateIdentityRooms();
        this.cutRoute(rooms, ROOM_REMOVE_DOOR_RATE);
        this.putMines(rooms);
        this.putItems(rooms, NUM_KEYS, ROOM_FLG_HAS_KEY);
        this.putItems(rooms, NUM_PLAYERS, ROOM_FLG_HAS_SPAWN);
        return rooms;
    }

    /// <summary>扉を指定の確率で削除します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="percentage">確率を 0f～1f の間で指定します。</param>
    private void cutRoute(byte[] rooms, float percentage)
    {
        var roomsCalculator = this.roomsCalculator;
        var amount =
            (int)(rooms.Length * this.constants.DIR_MAX * percentage);
        while (amount >= 0)
        {
            var dirs = this.directionCalculator.Direction;
            var targetIndex = Random.Range(0, rooms.Length);
            var currentRoom = rooms[targetIndex];
            var dirMark = ~(uint)dirs[Random.Range(0, dirs.Length)];
            var nextRoom = (byte)(currentRoom & dirMark);
            rooms[targetIndex] = nextRoom;
            var explorable =
                roomsCalculator.GetExplorableRoomsLength(rooms);
            var next = explorable == rooms.Length;
            if (next)
            {
                amount--;
            }
            else
            {
                rooms[targetIndex] = currentRoom;
            }
        }
    }

    /// <summary>地雷を設置します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    private void putMines(byte[] rooms)
    {
        var NUM_MINES = this.constants.NUM_MINES;
        var ROOM_FLG_HAS_MINE = this.constants.ROOM_FLG_HAS_MINE;
        var roomsCalculator = this.roomsCalculator;
        int putted = 0;
        while (putted < NUM_MINES)
        {
            var targetIndex = Random.Range(0, rooms.Length);
            var currentRoom = rooms[targetIndex];
            if ((currentRoom & ROOM_FLG_HAS_MINE) != 0)
            {
                continue;
            }
            var nextRoom = (byte)(currentRoom | ROOM_FLG_HAS_MINE);
            rooms[targetIndex] = nextRoom;
            var amount = rooms.Length - putted;
            var next =
                !roomsCalculator.HasUnExplorableMine(rooms) &&
                roomsCalculator.GetExplorableRoomsLength(rooms) == amount;
            if (next)
            {
                putted++;
            }
            else
            {
                rooms[targetIndex] = currentRoom;
            }
        }
    }

    /// <summary>アイテムを設置します。</summary>
    /// <param name="rooms">部屋情報一覧。</param>
    /// <param name="amount">設置個数。</param>
    /// <param name="flag">アイテムを示すフラグ。</param>
    private void putItems(byte[] rooms, int amount, int flag)
    {
        var roomsCalculator = this.roomsCalculator;
        while (amount > 0)
        {
            var targetIndex = Random.Range(0, rooms.Length);
            var currentRoom = rooms[targetIndex];
            if (roomsCalculator.HasAnyItems(currentRoom))
            {
                continue;
            }
            rooms[targetIndex] = (byte)(currentRoom | flag);
            amount--;
        }
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
    }
}
