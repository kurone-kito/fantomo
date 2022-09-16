using UdonSharp;
using UnityEngine;

/// <summary>フィールド算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FieldCalculator : UdonSharpBehaviour
{
    /// <summary>扉を削除する計算フェーズを示す定数。</summary>
    private const int CALC_PHASE_CUT_ROUTES = 0;

    /// <summary>地雷を設置する計算フェーズを示す定数。</summary>
    private const int CALC_PHASE_PUT_MINES = 1;

    /// <summary>鍵を設置する計算フェーズを示す定数。</summary>
    private const int CALC_PHASE_PUT_KEYS = 2;

    /// <summary>
    /// プレイヤーのスポーン地点を設置する計算フェーズを示す定数。
    /// </summary>
    private const int CALC_PHASE_PUT_SPAWNERS = 3;

    /// <summary>無効な計算フェーズを示す定数。</summary>
    private const int CALC_PHASE_DONE = 4;

    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    /// <summary>定数一覧。</summary>
    private Constants constants;

    /// <summary>方角周りの計算ロジック。</summary>
    private DirectionCalculator directionCalculator;

    /// <summary>初期化マネージャー コンポーネント。</summary>
    private InitializeManager initializeManager;

    /// <summary>部屋情報算出のロジック。</summary>
    private RoomsCalculator roomsCalculator;

    /// <summary>同期管理オブジェクト。</summary>
    private SyncManager syncManager;

    /// <summary>完了時に処理を戻すオブジェクト。</summary>
    private UdonSharpBehaviour callObjectOnComplete;

    /// <summary>完了時に処理を戻すメソッド。</summary>
    private string callMethodOnComplete;

    /// <summary>各部屋の計算状態。</summary>
    private byte[] rooms;

    /// <summary>各部屋の計算フェーズ。</summary>
    private int phase = -1;

    /// <summary>計算フェーズごとのカウント。</summary>
    private int phaseCount = 0;

    /// <summary>進捗率。</summary>
    private float progress = 0.0f;

    /// <summary>進捗率。</summary>
    public float Progress {
        get {
            float done = CALC_PHASE_DONE;
            return this.phase < 0
                ? 1f
                : this.phase / done + this.progress * (1f / done);
        }
        private set
        {
            progress = value;
            if (this.initializeManager != null)
            {
                this.initializeManager.RefreshProgressBar();
            }
            if (this.syncManager != null)
            {
                this.syncManager.ChangeOwner();
                this.syncManager.fieldCalculateProgress = this.Progress;
                this.syncManager.RequestSerialization();
            }
        }
    }

    /// <summary>各部屋の計算フェーズ。</summary>
    private int Phase {
        get => this.phase;
        set
        {
            this.phase = value;
            this.phaseCount = 0;
            this.progress = 0f;
        }
    }

    /// <summary>
    /// フィールドの計算をします。
    /// </summary>
    /// <param name="callObjectOnComplete">完了時に処理を戻すオブジェクト。</param>
    /// <param name="callMethodOnComplete">完了時に処理を戻すメソッド。</param>
    public void StartCalculate(
        UdonSharpBehaviour callObjectOnComplete,
        string callMethodOnComplete)
    {
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
        this.Phase = CALC_PHASE_CUT_ROUTES;
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
        switch (this.Phase)
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
            case CALC_PHASE_DONE:
                if (this.syncManager != null)
                {
                    this.syncManager.ChangeOwner();
                    this.syncManager.rooms = this.rooms;
                    this.syncManager.RequestSerialization();
                }
                if (
                    this.callMethodOnComplete != null &&
                    this.callObjectOnComplete != null)
                {
                    this.callObjectOnComplete.SendCustomEvent(
                        this.callMethodOnComplete);
                }
                toBeContinue = false;
                this.Phase = -1;
                break;
            default:
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
            var max =
                (int)(
                    this.rooms.Length *
                    this.constants.DIR_MAX *
                    this.constants.ROOM_REMOVE_DOOR_RATE);
            this.Progress = (float)(++this.phaseCount) / max;
            if (this.phaseCount >= max)
            {
                this.Phase = CALC_PHASE_PUT_MINES;
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
            this.Progress = (float)(++this.phaseCount) / NUM_MINES;
            if (this.phaseCount >= NUM_MINES)
            {
                this.Phase = CALC_PHASE_PUT_KEYS;
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
        this.Progress = (float)(++this.phaseCount) / items;
        if (this.phaseCount >= items)
        {
            this.Phase++;
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
            this.initializeManager =
                this.managers.GetComponentInChildren<InitializeManager>();
            this.roomsCalculator =
                this.managers.GetComponentInChildren<RoomsCalculator>();
            this.syncManager =
                this.managers.GetComponentInChildren<SyncManager>();
        }
    }
}
