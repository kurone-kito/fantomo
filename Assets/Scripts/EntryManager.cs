
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EntryManager : UdonSharpBehaviour
{
    /// <summary>同期管理オブジェクト。</summary>
    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    /// <summary>フィールド算出のロジック。</summary>
    private FieldCalculator fieldCalculator;

    /// <summary>同期管理オブジェクト。</summary>
    private SyncManager syncManager;

    /// <summary>エントリーしている、プレイヤーの一覧。。</summary>
    public short[] Ids => syncManager == null
        ? new short[0]
        : syncManager.playersId;

    /// <summary>参加メンバーが確定したかどうかを取得します。</summary>
    public bool Decided => syncManager != null && syncManager.decided;

    /// <summary>エントリー フォーム表示制御のロジック。</summary>
    public EntrySystem EntrySystem
    {
        get;
        set;
    }

    /// <summary>現在プレイしているプレイヤーの ID を取得します。</summary>
    public short LocalPlayerId => (short)Networking.LocalPlayer.playerId;

    /// <summary>現在のメンバーでゲームを開始します。</summary>
    public void Decide()
    {
        if (syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.Decide");
            return;
        }
        if (fieldCalculator == null)
        {
            Debug.LogError(
                "fieldCalculator が null のため、エントリーを行えません。: EntryManager.Decide");
            return;
        }
        syncManager.ChangeOwner();
        syncManager.decided = true;
        syncManager.RequestSerialization();
    }

    /// <summary>空きスロットのインデックスを取得します。</summary>
    /// <returns>
    /// 空きスロットのインデックス。存在しない場合、負数。
    /// </returns>
    public int GetEmpty()
    {
        var ids = Ids;
        for (var i = ids.Length; --i >= 0;)
        {
            var player = VRCPlayerApi.GetPlayerById(ids[i]);
            if (player == null || !player.IsValid())
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>エントリーしているかどうかを取得します。</summary>
    /// <returns>エントリーしている場合、<c>true</c>。</returns>
    public bool IsEntry()
    {
        return syncManager != null && Array.IndexOf(Ids, LocalPlayerId) >= 0;
    }

    /// <summary>
    /// 任意のプレイヤーがエントリーしているかどうかを取得します。
    /// </summary>
    /// <returns>エントリーしている場合、<c>true</c>。</returns>
    public bool IsEntryAny()
    {
        if (syncManager != null)
        {
            foreach (var id in Ids)
            {
                var player = VRCPlayerApi.GetPlayerById(id);
                if (player != null && player.IsValid())
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// ローカル プレイヤーを追加または削除します。
    /// 即ち、エントリー状態を切り替えます。
    /// </summary>
    public void ToggleEntry()
    {
        if (syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.AddOrRemoveLocalPlayer");
            return;
        }
        syncManager.ChangeOwner();
        if (IsEntry())
        {
            SYNC__removeId(LocalPlayerId);
        }
        else
        {
            SYNC__addId();
        }
        syncManager.RequestSerialization();
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        if (EntrySystem != null)
        {
            EntrySystem.UpdateView();
        }
    }

    /// <summary>
    /// <para>
    /// 任意のプレイヤーがリスポーンした際に呼び出す、コールバック。
    /// </para>
    /// <para>
    /// このワールドでは、リスポーンはリタイアと同義であるため、
    /// エントリーを強制的に取り消しています。
    /// </para>
    /// </summary>
    /// <param name="player">リスポーンしたプレイヤー。</param>
    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.OnPlayerRespawn");
            return;
        }
        if (player.isLocal)
        {
            syncManager.ChangeOwner();
            SYNC__removeId((short)player.playerId);
            if (!IsEntryAny())
            {
                syncManager.decided = false;
            }
            syncManager.RequestSerialization();
        }
        if (EntrySystem != null)
        {
            EntrySystem.UpdateView();
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
            syncManager =
                managers.GetComponentInChildren<SyncManager>();
            fieldCalculator =
                managers.GetComponentInChildren<FieldCalculator>();
        }
    }

    /// <summary>
    /// <para>プレイヤー ID をエントリー一覧に追加します。</para>
    /// <para>
    /// 同期変数の変更を行います。呼出し後に
    /// <seealso cref="UdonSharpBehaviour.RequestSerialization"/>
    /// メソッドを呼び出してください。
    /// </para>
    /// </summary>
    private void SYNC__addId()
    {
        Ids[GetEmpty()] = LocalPlayerId;
    }

    /// <summary>
    /// <para>プレイヤー ID をエントリー一覧から削除します。</para>
    /// <para>
    /// 同期変数の変更を行います。呼出し後に
    /// <seealso cref="UdonSharpBehaviour.RequestSerialization"/>
    /// メソッドを呼び出してください。
    /// </para>
    /// </summary>
    /// <param name="id">プレイヤー ID。</param>
    private void SYNC__removeId(short id)
    {
        var index = Array.IndexOf(Ids, id);
        if (index >= 0)
        {
            Ids[index] = 0;
        }
    }
}
