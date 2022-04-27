
using UdonSharp;
using UnityEngine;

/// <summary>
/// <para>初期化管理クラス 兼 初期化表示キャンバスのロジック。</para>
/// <para>
/// 必ず<seealso cref="UdonSharpBehaviour"/>を継承するという U# 制約のため、
/// 若干不格好ですが初期化表示キャンバスに寄生する形で実装しています。
/// </para>
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InitializeManager : UdonSharpBehaviour
{
    /// <value>鍵数。</value>
    private const int KEYS_NUM = 10;

    /// <value>地雷数。</value>
    private const int MINES_NUM = 9;

    /// <value>部屋数。</value>
    private const int ROOMS_NUM = 64;

    /// <value>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>すべき、
    /// ソースとなるオブジェクト一覧の要素数。
    /// </value>
    private const int SOURCES_LENGTH = KEYS_NUM + MINES_NUM + ROOMS_NUM + 2;

    [Header("Lobby room")]

    /// <value>エントリーフォーム オブジェクト。</value>
    [SerializeField]
    private GameObject entrySystem;

    /// <value>ロビールーム オブジェクト。</value>
    [SerializeField]
    private GameObject lobbyRoom;

    /// <value>ミラー表示 オブジェクト。</value>
    [SerializeField]
    private GameObject mirrorSystem;

    /// <value>プログレス バー コンポーネント。</value>
    [SerializeField]
    private InitialGameProgress progress;

    [Header("Game Field")]

    /// <value>鍵オブジェクト。</value>
    [SerializeField]
    private GameObject key;

    /// <value>地雷オブジェクト。</value>
    [SerializeField]
    private GameObject mine;

    /// <value>部屋オブジェクト。</value>
    [SerializeField]
    private GameObject room;

    /// <value>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>すべき、
    /// ソースとなるオブジェクト一覧。
    /// </value>
    private GameObject[] sources = new GameObject[SOURCES_LENGTH];

    /// <value>
    /// <seealso cref="InitializeManager.sources"/>に対する、
    /// 所属する親の一覧。
    /// </value>
    private GameObject[] parents = new GameObject[SOURCES_LENGTH];

    /// <value>
    /// <seealso cref="InitializeManager.sources"/>に対する、
    /// 生成先座標の一覧。
    /// </value>
    private Vector3[] positions = new Vector3[SOURCES_LENGTH];

    /// <value>
    /// <seealso cref="InitializeManager.sources"/>に対する、
    /// イテレーターを示すインデックス。
    /// </value>
    private int iterator = 0;

    /// <summary>
    /// 初期化を開始します。
    /// </summary>
    public void StartInitialize()
    {
        this.parents[0] = this.lobbyRoom;
        this.positions[0] = new Vector3(0f, 1.4f, -4.8f);
        this.sources[0] = this.mirrorSystem;
        this.parents[1] = this.lobbyRoom;
        this.positions[1] = new Vector3(4.94f, 1.4f, 0f);
        this.sources[1] = this.entrySystem;
        var index = 2;
        for (var i = 0; i < KEYS_NUM; i++)
        {
            this.sources[index++] = this.key;
        }
        for (var i = 0; i < MINES_NUM; i++)
        {
            this.sources[index++] = this.mine;
        }
        // for (var i = 0; i < ROOMS_NUM; i++)
        // {
        //     this.sources[index++] = this.room;
        // }
        this.SendCustomEventDelayedSeconds("RunInstantiateIteration", .1f);
    }

    public void RunInstantiateIteration()
    {
        if (this.iterator < this.sources.Length)
        {
            var i = this.iterator++;
            var src = this.sources[i];
            if (src != null)
            {
                var obj = UdonSharpBehaviour.VRCInstantiate(src);
                obj.transform.position = this.positions[i];
                if (i == 0)
                {
                    obj.transform.Rotate(0f, 180f, 0f);
                }
                var parent = this.parents[i];
                if (parent != null)
                {
                    obj.transform.parent = parent.transform;
                }
            }
            this.progress.Progress = (float)this.iterator / this.sources.Length;
            this.SendCustomEventDelayedSeconds("RunInstantiateIteration", .1f);
        }
        else
        {
            this.progress.Progress = 1f;
        }
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.progress)
        {
            this.SendCustomEventDelayedSeconds("StartInitialize", 1f);
        }
    }
}
