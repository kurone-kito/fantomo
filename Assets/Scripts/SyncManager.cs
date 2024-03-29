﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

/// <summary>同期機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncManager : UdonSharpBehaviour
{
    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    /// <summary>エントリー機能ロジック。</summary>
    private EntryManager entryManager;

    /// <summary>参加メンバーが確定したかどうか。</summary>
    [NonSerialized]
    [UdonSynced]
    public bool decided = false;

    /// <summary>フィールド再計算の進捗状況。</summary>
    [NonSerialized]
    [UdonSynced]
    public float fieldCalculateProgress = 0f;

    /// <summary>鍵の配置インデックス一覧。</summary>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] keys = new sbyte[0];

    /// <summary>ドアをロックしている、プレイヤー ID。</summary>
    [NonSerialized]
    [UdonSynced]
    public short[] locked = new short[0];

    /// <summary>地雷の配置インデックス一覧。</summary>
    [NonSerialized]
    [UdonSynced]
    public sbyte[] mines = new sbyte[0];

    /// <summary>X 軸側のドアが開いているかどうか。</summary>
    [NonSerialized]
    [UdonSynced]
    public ulong openedX = 0ul;

    /// <summary>Y 軸側のドアが開いているかどうか。</summary>
    [NonSerialized]
    [UdonSynced]
    public ulong openedY = 0ul;

    /// <summary>エントリーしている、プレイヤーの一覧。</summary>
    [NonSerialized]
    [UdonSynced]
    public short[] playersId = new short[0];

    /// <summary>部屋情報一覧。</summary>
    [NonSerialized]
    [UdonSynced]
    public byte[] rooms = new byte[0];

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.decided"/>の値。
    /// </summary>
    public bool PrevDecided
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.fieldCalculateProgress"/>の値。
    /// </summary>
    public float PrevFieldCalculateProgress
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.keys"/>の値。
    /// </summary>
    public sbyte[] PrevKeys
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.locked"/>の値。
    /// </summary>
    public short[] PrevLocked
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.mines"/>の値。
    /// </summary>
    public sbyte[] PrevMines
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.openedX"/>の値。
    /// </summary>
    public ulong PrevOpenedX
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.openedY"/>の値。
    /// </summary>
    public ulong PrevOpenedY
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.playersId"/>の値。
    /// </summary>
    public short[] PrevPlayersId
    {
        get;
        private set;
    }

    /// <summary>
    /// 前回同期時の<seealso cref="SyncManager.rooms"/>の値。
    /// </summary>
    public byte[] PrevRooms
    {
        get;
        private set;
    }

    /// <summary>オブジェクトオーナーを奪取・変更します。</summary>
    public void ChangeOwner()
    {
        var player = Networking.LocalPlayer;
        if (player != null && !Networking.IsOwner(player, gameObject))
        {
            Networking.SetOwner(player, gameObject);
        }
    }

    /// <summary>ゲーム開始を全プレイヤーに伝達します。</summary>
    public void GameStart()
    {
        ChangeOwner();
        decided = true;
        RequestSerialization();
        SendCustomNetworkEvent(
            NetworkEventTarget.All, nameof(OnDecidedGameStart));
    }

    /// <summary>
    /// ゲーム開始を各プレイヤーが認知した時のコールバック。
    /// </summary>
    public void OnDecidedGameStart()
    {
        entryManager.EntrySystem.TeleportToGameField();
    }

    /// <summary>
    /// 同期データの送信直後に呼び出す、コールバック。
    /// </summary>
    public override void OnPostSerialization(SerializationResult result)
    {
        OnDeserialization();
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        if (entryManager != null)
        {
            entryManager.OnDeserialization();
        }
        StoreValues();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (managers != null)
        {
            entryManager = managers.GetComponentInChildren<EntryManager>();
        }
        keys = new sbyte[Constants.NUM_KEYS];
        locked = new short[Constants.NUM_ROOMS];
        mines = new sbyte[Constants.NUM_MINES];
        playersId = new short[Constants.NUM_PLAYERS];
        rooms = new byte[Constants.NUM_ROOMS];
        StoreValues();
    }

    /// <summary>
    /// 現在の値を、以前の値として保持します。
    /// </summary>
    private void StoreValues()
    {
        PrevDecided = decided;
        PrevFieldCalculateProgress = fieldCalculateProgress;
        PrevKeys = keys;
        PrevLocked = locked;
        PrevMines = mines;
        PrevOpenedX = openedX;
        PrevOpenedY = openedY;
        PrevPlayersId = playersId;
        PrevRooms = rooms;
    }
}
