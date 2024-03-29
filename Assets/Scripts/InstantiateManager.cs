﻿
using UdonSharp;
using UnityEngine;

/// <summary>
/// 動的なゲームオブジェクトを非同期的に Instantiate するロジック。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InstantiateManager : UdonSharpBehaviour
{
    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    [Header("Lobby room")]

    /// <summary>エントリーフォーム オブジェクト。</summary>
    [SerializeField]
    private GameObject entrySystem;

    /// <summary>ロビールーム オブジェクト。</summary>
    [SerializeField]
    private GameObject lobbyRoom;

    /// <summary>ミラー表示 オブジェクト。</summary>
    [SerializeField]
    private GameObject mirrorSystem;

    [Header("Game Field")]

    /// <summary>ゲーム フィールド オブジェクト。</summary>
    [SerializeField]
    private GameField gameField;

    /// <summary>鍵オブジェクト。</summary>
    [SerializeField]
    private GameObject key;

    /// <summary>地雷オブジェクト。</summary>
    [SerializeField]
    private GameObject mine;

    /// <summary>部屋オブジェクト。</summary>
    [SerializeField]
    private GameObject room;

    /// <summary>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>によって生成された、
    /// オブジェクト マネージャー。
    /// </summary>
    private ClonesManager clonesManager;

    /// <summary>初期化マネージャー コンポーネント。</summary>
    private InitializeManager initializeManager;

    /// <summary>
    /// <seealso cref="InstantiateManager.sources"/>に対する、
    /// 生成物のインデックス一覧。
    /// </summary>
    private int[] indexes;

    /// <summary>
    /// <seealso cref="InstantiateManager.sources"/>に対する、
    /// イテレーターを示すインデックス。
    /// </summary>
    private int iterator = 0;

    /// <summary>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>すべき、
    /// ソースとなるオブジェクト一覧。
    /// </summary>
    private GameObject[] sources;

    /// <summary>
    /// <seealso cref="InstantiateManager.sources"/>に対する、
    /// 所属する親の一覧。
    /// </summary>
    private GameObject[] parents;

    /// <summary>
    /// <seealso cref="InstantiateManager.sources"/>に対する、
    /// 生成先座標の一覧。
    /// </summary>
    private Vector3[] positions;

    /// <summary>完了時に処理を戻すオブジェクト。</summary>
    private UdonSharpBehaviour callObjectOnComplete;

    /// <summary>完了時に処理を戻すメソッド。</summary>
    private string callMethodOnComplete;

    /// <summary>進捗率。</summary>
    private float progress = 0.0f;

    /// <summary>進捗率。</summary>
    public float Progress
    {
        get => progress;
        private set
        {
            progress = value;
            if (this.initializeManager != null)
            {
                this.initializeManager.RefreshProgressBar();
            }
        }
    }

    /// <summary>初期化を開始します。</summary>
    /// <param name="callObjectOnComplete">完了時に処理を戻すオブジェクト。</param>
    /// <param name="callMethodOnComplete">完了時に処理を戻すメソッド。</param>
    public void StartBatchInstantiate(
        UdonSharpBehaviour callObjectOnComplete,
        string callMethodOnComplete)
    {
        if (this.initializeManager == null)
        {
            Debug.LogError(
                "initializeManager が null のため、Instantiate できません。: InstantiateManager.StartBatchInstantiate");
            return;
        }
        var LOAD_INTERVAL = Constants.LOAD_INTERVAL;
        var NUM_INSTANTIATES = Constants.NUM_INSTANTIATES;
        var NUM_KEYS = Constants.NUM_KEYS;
        var NUM_MINES = Constants.NUM_MINES;
        var NUM_ROOMS = Constants.NUM_ROOMS;
        var ROOM_SIZE = Constants.ROOM_SIZE;
        var ROOMS_WIDTH = Constants.ROOMS_WIDTH;
        this.callMethodOnComplete = callMethodOnComplete;
        this.callObjectOnComplete = callObjectOnComplete;
        this.indexes = new int[NUM_INSTANTIATES];
        this.sources = new GameObject[NUM_INSTANTIATES];
        this.parents = new GameObject[NUM_INSTANTIATES];
        this.positions = new Vector3[NUM_INSTANTIATES];
        this.parents[0] = this.lobbyRoom;
        this.positions[0] = new Vector3(4.94f, 1.4f, 0f);
        this.sources[0] = this.entrySystem;
        this.parents[1] = this.lobbyRoom;
        this.positions[1] = new Vector3(0f, 1.4f, -4.8f);
        this.sources[1] = this.mirrorSystem;
        var index = 2;
        for (var i = 0; i < NUM_KEYS; i++)
        {
            var j = index++;
            this.indexes[j] = i;
            this.sources[j] = this.key;
            this.parents[j] = this.gameField ? this.gameField.gameObject : null;
        }
        for (var i = 0; i < NUM_MINES; i++)
        {
            var j = index++;
            this.indexes[j] = i;
            this.sources[j] = this.mine;
            this.parents[j] = this.gameField ? this.gameField.gameObject : null;
        }
        for (var i = 0; i < NUM_ROOMS; i++)
        {
            var j = index++;
            this.indexes[j] = i;
            this.sources[j] = this.room;
            this.parents[j] = this.gameField ? this.gameField.gameObject : null;
            this.positions[j] =
                Vector3.right * (i % ROOMS_WIDTH) * ROOM_SIZE +
                Vector3.forward * (i / ROOMS_WIDTH) * ROOM_SIZE;
        }
        this.SendCustomEventDelayedSeconds(
            nameof(RunInstantiateIteration),
            LOAD_INTERVAL);
    }

    /// <summary>1 イテレーションごとの初期化をします。</summary>
    public void RunInstantiateIteration()
    {
        var LOAD_INTERVAL = Constants.LOAD_INTERVAL;
        if (this.iterator < this.sources.Length)
        {
            var i = this.iterator++;
            var src = this.sources[i];
            if (src != null)
            {
                var obj = Instantiate(src);
                var parent = this.parents[i];
                if (parent != null)
                {
                    obj.transform.parent = parent.transform;
                }
                obj.transform.localPosition = this.positions[i];
                if (src == this.entrySystem)
                {
                    this.SetSyncManagerToEntrySystem();
                }
                if (src == this.mirrorSystem)
                {
                    obj.transform.Rotate(0f, 180f, 0f);
                }
                if (src == this.room && this.gameField != null)
                {
                    this.gameField.rooms[this.indexes[i]] = obj;
                }
                if (
                    src == this.room &&
                    this.clonesManager != null &&
                    this.clonesManager.Rooms != null
                )
                {
                    this.clonesManager.Rooms[this.indexes[i]] = obj;
                }
                if (
                    src == this.key &&
                    this.clonesManager != null &&
                    this.clonesManager.Keys != null
                )
                {
                    this.clonesManager.Keys[this.indexes[i]] = obj;
                }
                if (
                    src == this.mine &&
                    this.clonesManager != null &&
                    this.clonesManager.Mines != null
                )
                {
                    this.clonesManager.Mines[this.indexes[i]] = obj;
                }
            }
            this.Progress = (float)this.iterator / this.sources.Length;
            this.SendCustomEventDelayedSeconds(
                nameof(RunInstantiateIteration),
                LOAD_INTERVAL);
        }
        else
        {
            this.SendCustomEventDelayedSeconds(
                nameof(FinishInstantiate),
                LOAD_INTERVAL);
        }
    }

    /// <summary>
    /// すべてのイテレーション完了後に、
    /// ゲームフィールド初期化処理を引き継ぎます。
    /// </summary>
    public void FinishInstantiate()
    {
        var entrySystem = this.lobbyRoom.GetComponentInChildren<EntrySystem>();
        if (entrySystem)
        {
            this.gameField.Initialize();
            entrySystem.gameField = this.gameField;
            entrySystem.UpdateView();
            this.Progress = 1f;
            if (
                this.callMethodOnComplete != null &&
                this.callObjectOnComplete != null)
            {
                this.callObjectOnComplete.SendCustomEvent(
                    this.callMethodOnComplete);
            }
        }
        else
        {
            this.SendCustomEventDelayedSeconds(
                nameof(FinishInstantiate),
                Constants.LOAD_INTERVAL);
        }
    }

    /// <summary>
    /// エントリー フォーム オブジェクトに対し、
    /// 同期マネージャーを設定します。
    /// </summary>
    public void SetSyncManagerToEntrySystem()
    {
        if (this.managers == null)
        {
            Debug.LogError(
                "managers が null のため、初期化を行えません。: InstantiateManager.SetSyncManagerToEntrySystem");
            return;
        }
        var entrySystem = this.lobbyRoom.GetComponentInChildren<EntrySystem>();
        if (entrySystem)
        {
            entrySystem.EntryManagerInstance =
                this.managers.GetComponentInChildren<EntryManager>();
            entrySystem.UpdateView();
            this.initializeManager.EntrySystem = entrySystem;
        }
        else
        {
            this.SendCustomEventDelayedSeconds(nameof(SetSyncManagerToEntrySystem), .1f);
        }
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers)
        {
            this.clonesManager =
                this.managers.GetComponentInChildren<ClonesManager>();
            this.initializeManager =
                this.managers.GetComponentInChildren<InitializeManager>();
        }
    }
}
