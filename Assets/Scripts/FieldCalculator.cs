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
    public float Progress
    {
        get
        {
            float done = CALC_PHASE_DONE;
            return phase < 0
                ? 1f
                : (phase / done) + (progress * (1f / done));
        }
        private set
        {
            progress = value;
            if (initializeManager != null)
            {
                initializeManager.RefreshProgressBar();
            }
            if (syncManager != null)
            {
                syncManager.ChangeOwner();
                syncManager.fieldCalculateProgress = Progress;
                syncManager.RequestSerialization();
            }
        }
    }

    /// <summary>各部屋の計算フェーズ。</summary>
    private int Phase
    {
        get => phase;
        set
        {
            phase = value;
            phaseCount = 0;
            progress = 0f;
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
        if (roomsCalculator == null)
        {
            Debug.LogError(
                "roomsCalculator が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return;
        }
        var LOAD_INTERVAL = Constants.LOAD_INTERVAL;
        this.callMethodOnComplete = callMethodOnComplete;
        this.callObjectOnComplete = callObjectOnComplete;
        rooms = roomsCalculator.CreateIdentityRooms();
        Phase = CALC_PHASE_CUT_ROUTES;
        SendCustomEventDelayedSeconds(
            nameof(RunIteration),
            LOAD_INTERVAL);
    }

    /// <summary>
    /// 1 イテレーションごとの処理をします。
    /// </summary>
    public void RunIteration()
    {
        var LOAD_INTERVAL = Constants.LOAD_INTERVAL;
        var toBeContinue = true;
        switch (Phase)
        {
            case CALC_PHASE_CUT_ROUTES:
                cutRoute();
                break;
            case CALC_PHASE_PUT_MINES:
                putMines();
                break;
            case CALC_PHASE_PUT_KEYS:
                putItems(
                    Constants.NUM_KEYS,
                    (byte)ROOM_FLG.HAS_KEY);
                break;
            case CALC_PHASE_PUT_SPAWNERS:
                putItems(
                    Constants.NUM_PLAYERS,
                    (byte)ROOM_FLG.HAS_SPAWN);
                break;
            case CALC_PHASE_DONE:
                if (syncManager != null)
                {
                    syncManager.ChangeOwner();
                    syncManager.rooms = rooms;
                    syncManager.RequestSerialization();
                }
                if (
                    callMethodOnComplete != null &&
                    callObjectOnComplete != null)
                {
                    callObjectOnComplete.SendCustomEvent(
                        callMethodOnComplete);
                }
                toBeContinue = false;
                Phase = -1;
                break;
            default:
                toBeContinue = false;
                break;
        }
        if (toBeContinue)
        {
            SendCustomEventDelayedSeconds(
                nameof(RunIteration),
                LOAD_INTERVAL);
        }
    }

    /// <summary>扉を指定の確率で削除します。</summary>
    private void cutRoute()
    {
        var rooms = this.rooms;
        var dirs = directionCalculator.Direction;
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
                    (int)DIR.MAX *
                    Constants.ROOM_REMOVE_DOOR_RATE);
            Progress = (float)(++phaseCount) / max;
            if (phaseCount >= max)
            {
                Phase = CALC_PHASE_PUT_MINES;
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
        var NUM_MINES = Constants.NUM_MINES;
        var rooms = this.rooms;
        var roomsCalculator = this.roomsCalculator;
        var targetIndex = Random.Range(0, rooms.Length);
        var currentRoom = rooms[targetIndex];
        if ((currentRoom & (byte)ROOM_FLG.HAS_MINE) != 0)
        {
            return;
        }
        var nextRoom = (byte)(currentRoom | (byte)ROOM_FLG.HAS_MINE);
        rooms[targetIndex] = nextRoom;
        var amount = rooms.Length - phaseCount;
        var next =
            !roomsCalculator.HasUnReachableMine(rooms) &&
            roomsCalculator.GetReachableRoomsLength(rooms) == amount;
        if (next)
        {
            Progress = (float)(++phaseCount) / NUM_MINES;
            if (phaseCount >= NUM_MINES)
            {
                Phase = CALC_PHASE_PUT_KEYS;
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
        Progress = (float)(++phaseCount) / items;
        if (phaseCount >= items)
        {
            Phase++;
        }
    }

    /// <summary>
    /// <para>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </para>
    /// <para>ここでは、各フィールドの確保を行います。</para>
    /// </summary>
    private void Start()
    {
        if (managers)
        {
            directionCalculator =
                managers.GetComponentInChildren<DirectionCalculator>();
            initializeManager =
                managers.GetComponentInChildren<InitializeManager>();
            roomsCalculator =
                managers.GetComponentInChildren<RoomsCalculator>();
            syncManager =
                managers.GetComponentInChildren<SyncManager>();
        }
    }
}
