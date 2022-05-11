
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

/// <summary>同期機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncManager : UdonSharpBehaviour
{
    /// <value>管理ロジックの親となるオブジェクト。</value>
    [SerializeField]
    private GameObject managers;

    /// <value>定数一覧。</value>
    private Constants constants;

    /// <value>エントリー機能ロジック。</value>
    private EntryManager entryManager;

    /// <value>参加メンバーが確定したかどうか。</value>
    [NonSerialized]
    [UdonSynced]
    public bool decided = false;

    /// <value>鍵の配置インデックス一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] keys = new sbyte[0];

    /// <value>ドアをロックしている、プレイヤー ID。</value>
    [NonSerialized]
    [UdonSynced]
    public short[] locked = new short[0];

    /// <value>地雷の配置インデックス一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] mines = new sbyte[0];

    /// <value>X 軸側のドアが開いているかどうか。</value>
    [NonSerialized]
    [UdonSynced]
    public ulong openedX = 0ul;

    /// <value>Y 軸側のドアが開いているかどうか。</value>
    [NonSerialized]
    [UdonSynced]
    public ulong openedY = 0ul;

    /// <value>エントリーしている、プレイヤーの一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public short[] playersId = new short[0];

    /// <value>前回同期時の<seealso cref="SyncManager.decided"/>の値。</value>
    public bool prevDecided
    {
        get;
        private set;
    }

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
    /// 前回同期時の<seealso cref="SyncManager.openedX"/>の値。
    /// </value>
    public ulong prevOpenedX
    {
        get;
        private set;
    }

    /// <value>
    /// 前回同期時の<seealso cref="SyncManager.openedY"/>の値。
    /// </value>
    public ulong prevOpenedY
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
    /// 同期データの送信直後に呼び出す、コールバック。
    /// </summary>
    public override void OnPostSerialization(SerializationResult result)
    {
        this.OnDeserialization();
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        if (this.entryManager != null)
        {
            this.entryManager.OnDeserialization();
        }
        this.storeValues();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers != null)
        {
            this.constants = this.managers.GetComponentInChildren<Constants>();
            this.entryManager = this.managers.GetComponentInChildren<EntryManager>();
        }
        if (this.constants != null)
        {
            this.keys = new sbyte[this.constants.NUM_KEYS];
            this.locked = new short[this.constants.NUM_ROOMS];
            this.mines = new sbyte[this.constants.NUM_MINES];
            this.playersId = new short[this.constants.NUM_PLAYERS];
        }
        this.storeValues();
    }

    /// <summary>
    /// 現在の値を、以前の値として保持します。
    /// </summary>
    private void storeValues()
    {
        this.prevDecided = this.decided;
        this.prevKeys = this.keys;
        this.prevLocked = this.locked;
        this.prevMines = this.mines;
        this.prevOpenedX = this.openedX;
        this.prevOpenedY = this.openedY;
        this.prevPlayersId = this.playersId;
    }
}
