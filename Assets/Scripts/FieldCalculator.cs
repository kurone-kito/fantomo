using UdonSharp;
using UnityEngine;

/// <summary>フィールド算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FieldCalculator : UdonSharpBehaviour
{
    /// <value>扉を削除する計算フェーズを示す定数。</value>
    private const int CALC_PHASE_CUT_ROUTES = 0;
    /// <value>地雷を設置する計算フェーズを示す定数。</value>
    private const int CALC_PHASE_PUT_MINES = 1;
    /// <value>鍵を設置する計算フェーズを示す定数。</value>
    private const int CALC_PHASE_PUT_KEYS = 2;
    /// <value>
    /// プレイヤーのスポーン地点を設置する計算フェーズを示す定数。
    /// </value>
    private const int CALC_PHASE_PUT_SPAWNERS = 3;
    /// <value>無効な計算フェーズを示す定数。</value>
    private const int CALC_PHASE_DONE = 4;
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
    /// <summary>完了時に処理を戻すオブジェクト。</summary>
    private UdonSharpBehaviour callObjectOnComplete;
    /// <summary>完了時に処理を戻すメソッド。</summary>
    private string callMethodOnComplete;
    /// <value>各部屋の計算状態。</value>
    private byte[] rooms;
    /// <value>各部屋の計算フェーズ。</value>
    private int phase = -1;
    /// <value>計算フェーズごとのカウント。</value>
    private int phaseCount = 0;

    /// <summary>
    /// フィールドの計算をします。
    /// </summary>
    /// <param name="callObjectOnComplete">完了時に処理を戻すオブジェクト。</param>
    /// <param name="callMethodOnComplete">完了時に処理を戻すメソッド。</param>
    public void Calculate(
        UdonSharpBehaviour callObjectOnComplete,
        string callMethodOnComplete)
    {
        Debug.Log("FieldCalculator.Calculate()");
        if (this.constants == null)
        {
            Debug.LogError(
                "constants が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return;
        }
        if (this.roomsCalculator == null)
        {
            Debug.LogError(
                "roomsCalculator が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return;
        }
        var LOAD_INTERVAL = this.constants.LOAD_INTERVAL;
        this.callMethodOnComplete = callMethodOnComplete;
        this.callObjectOnComplete = callObjectOnComplete;
        this.rooms = roomsCalculator.CreateIdentityRooms();
        this.initializeCutRoute();
        this.SendCustomEventDelayedSeconds(
            nameof(RunIteration),
            LOAD_INTERVAL);
    }

    /// <summary>
    /// 1 イテレーションごとの処理をします。
    /// </summary>
    public void RunIteration()
    {
        var LOAD_INTERVAL = this.constants.LOAD_INTERVAL;
        var toBeContinue = true;
        switch (this.phase)
        {
            case CALC_PHASE_CUT_ROUTES:
                this.cutRoute();
                break;
            case CALC_PHASE_PUT_MINES:
                this.putMines();
                break;
            case CALC_PHASE_PUT_KEYS:
                this.putItems(
                    this.constants.NUM_KEYS,
                    this.constants.ROOM_FLG_HAS_KEY);
                break;
            case CALC_PHASE_PUT_SPAWNERS:
                this.putItems(
                    this.constants.NUM_PLAYERS,
                    this.constants.ROOM_FLG_HAS_SPAWN);
                break;
            default:
                Debug.Log("フィールドを算出しました。");
                toBeContinue = false;
                break;
        }
        if (toBeContinue)
        {
            this.SendCustomEventDelayedSeconds(
                nameof(RunIteration),
                LOAD_INTERVAL);
        }
    }

    private void initializeCutRoute()
    {
        var ROOM_REMOVE_DOOR_RATE = this.constants.ROOM_REMOVE_DOOR_RATE;
        this.phase = CALC_PHASE_CUT_ROUTES;
        this.phaseCount =
            (int)(rooms.Length * this.constants.DIR_MAX * ROOM_REMOVE_DOOR_RATE);
    }

    /// <summary>扉を指定の確率で削除します。</summary>
    private void cutRoute()
    {
        var rooms = this.rooms;
        var dirs = this.directionCalculator.Direction;
        var targetIndex = Random.Range(0, rooms.Length);
        var currentRoom = rooms[targetIndex];
        var dirMark = ~(uint)dirs[Random.Range(0, dirs.Length)];
        var nextRoom = (byte)(currentRoom & dirMark);
        rooms[targetIndex] = nextRoom;
        var reachable =
            roomsCalculator.GetReachableRoomsLength(rooms);
        var next = reachable == rooms.Length;
        if (next)
        {
            if (--this.phaseCount < 0)
            {
                this.phase = CALC_PHASE_PUT_MINES;
                this.phaseCount = 0;
            }
        }
        else
        {
            rooms[targetIndex] = currentRoom;
        }
    }

    /// <summary>地雷を設置します。</summary>
    private void putMines()
    {
        var NUM_MINES = this.constants.NUM_MINES;
        var ROOM_FLG_HAS_MINE = this.constants.ROOM_FLG_HAS_MINE;
        var rooms = this.rooms;
        var roomsCalculator = this.roomsCalculator;
        var targetIndex = Random.Range(0, rooms.Length);
        var currentRoom = rooms[targetIndex];
        if ((currentRoom & ROOM_FLG_HAS_MINE) != 0)
        {
            return;
        }
        var nextRoom = (byte)(currentRoom | ROOM_FLG_HAS_MINE);
        rooms[targetIndex] = nextRoom;
        var amount = rooms.Length - this.phaseCount;
        var next =
            !roomsCalculator.HasUnReachableMine(rooms) &&
            roomsCalculator.GetReachableRoomsLength(rooms) == amount;
        if (next)
        {
            if (++this.phaseCount >= NUM_MINES)
            {
                this.phase = CALC_PHASE_PUT_KEYS;
                this.phaseCount = 0;
            }
        }
        else
        {
            rooms[targetIndex] = currentRoom;
        }
    }

    /// <summary>アイテムを設置します。</summary>
    /// <param name="items">設置個数。</param>
    /// <param name="flag">アイテムを示すフラグ。</param>
     private void putItems(int items, int flag)
    {
        var rooms = this.rooms;
        var roomsCalculator = this.roomsCalculator;
        var targetIndex = Random.Range(0, rooms.Length);
        var currentRoom = rooms[targetIndex];
        if (roomsCalculator.HasAnyItems(currentRoom))
        {
            return;
        }
        rooms[targetIndex] = (byte)(currentRoom | flag);
        if (++this.phaseCount >= items)
        {
            if (++this.phase == CALC_PHASE_DONE)
            {
                this.phase = -1;
                this.phaseCount = 0;
            }
            this.phaseCount = 0;
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
            this.constants =
                this.managers.GetComponentInChildren<Constants>();
            this.directionCalculator =
                this.managers.GetComponentInChildren<DirectionCalculator>();
            this.roomsCalculator =
                this.managers.GetComponentInChildren<RoomsCalculator>();
            this.syncManager =
                this.managers.GetComponentInChildren<SyncManager>();
        }
    }
}
