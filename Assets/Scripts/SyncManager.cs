
using System;
using UdonSharp;
using VRC.SDKBase;

/// <summary>同期機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncManager : UdonSharpBehaviour
{
    /// <value>最大エントリー可能数。</value>
    private const int MAX_PLAYERS = 3;

    /// <value>部屋数。</value>
    private const int ROOMS = 64;

    /// <value>地雷数。</value>
    private const int MINES = 10;

    /// <value>鍵の数。</value>
    private const int KEYS = 10;

    /// <value>鍵の配置インデックス一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] keys = new sbyte[KEYS];

    /// <value>ドアをロックしている、プレイヤー ID。</value>
    [NonSerialized]
    [UdonSynced]
    public short[] locked = new short[ROOMS];

    /// <value>地雷の配置インデックス一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] mines = new sbyte[MINES];

    /// <value>エントリーしている、プレイヤーの一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public short[] playersId = new short[MAX_PLAYERS];

    /// <value>前回同期時の<seealso cref="SyncManager.keys"/>の値。</value>
    public sbyte[] prevKeys
    {
        get;
        private set;
    }

    /// <value>
    /// 前回同期時の<seealso cref="SyncManager.locked"/>の値。
    /// </value>
    public short[] prevLocked
    {
        get;
        private set;
    }

    /// <value>
    /// 前回同期時の<seealso cref="SyncManager.mines"/>の値。
    /// </value>
    public sbyte[] prevMines
    {
        get;
        private set;
    }

    /// <value>
    /// 前回同期時の<seealso cref="SyncManager.playersId"/>の値。
    /// </value>
    public short[] prevPlayersId
    {
        get;
        private set;
    }

    /// <summary>オブジェクトオーナーを奪取・変更します。</summary>
    public void ChangeOwner()
    {
        var player = Networking.LocalPlayer;
        if (player != null && !Networking.IsOwner(player, this.gameObject))
        {
            Networking.SetOwner(player, this.gameObject);
        }
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        // TODO: Notify
        this.storeValues();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.storeValues();
    }

    /// <summary>
    /// 現在の値を、以前の値として保持します。
    /// </summary>
    private void storeValues()
    {
        this.prevKeys = this.keys;
        this.prevLocked = this.locked;
        this.prevMines = this.mines;
        this.prevPlayersId = this.playersId;
    }
}
