
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EntryManager : UdonSharpBehaviour
{
    /// <value>同期管理オブジェクト。</value>
    [SerializeField]
    private SyncManager syncManager;

    /// <value>エントリーしている、プレイヤーの一覧。。</value>
    public short[] Ids
    {
        get =>
            this.syncManager == null
                ? new short[0]
                : this.syncManager.playersId;
    }

    /// <value>参加メンバーが確定したかどうかを取得します。</value>
    public bool Decided
    {
        get => this.syncManager != null && this.syncManager.decided;
    }

    /// <summary>エントリー フォーム表示制御のロジック。</summary>
    public EntrySystem entrySystem
    {
        get;
        set;
    }

    /// <value>
    /// 現在プレイしているプレイヤーが無効であるかどうかを取得します。
    /// </value>
    public bool InvalidLocalPlayer
    {
        get => Networking.LocalPlayer == null;
    }

    /// <value>現在プレイしているプレイヤーの ID を取得します。</value>
    public short LocalPlayerId
    {
        get =>
            this.InvalidLocalPlayer
                ? short.MaxValue
                : (short)Networking.LocalPlayer.playerId;
    }

    /// <summary>現在のメンバーでゲームを開始します。</summary>
    public void Decide()
    {
        if (this.syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.Decide");
            return;
        }
        this.syncManager.ChangeOwner();
        this.syncManager.decided = true;
        this.syncManager.RequestSerialization();
    }

    /// <summary>空きスロットのインデックスを取得します。</summary>
    /// <returns>
    /// 空きスロットのインデックス。存在しない場合、負数。
    /// </returns>
    public int GetEmpty()
    {
        if (this.InvalidLocalPlayer)
        {
            return 0;
        }
        var localId = this.LocalPlayerId;
        var ids = this.Ids;
        for (var i = ids.Length; --i >= 0; )
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
    public bool IsEntried()
    {
        if (this.syncManager != null)
        {
            var localId = this.LocalPlayerId;
            foreach (var id in this.Ids)
            {
                if (id == localId)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 任意のプレイヤーがエントリーしているかどうかを取得します。
    /// </summary>
    /// <returns>エントリーしている場合、<c>true</c>。</returns>
    public bool IsEntriedAny()
    {
        if (this.syncManager != null)
        {
            foreach (var id in this.Ids)
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
        if (this.syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.AddOrRemoveLocalPlayer");
            return;
        }
        this.syncManager.ChangeOwner();
        if (this.IsEntried())
        {
            this.UNSYNC__removeId((short)this.LocalPlayerId);
        }
        else
        {
            this.UNSYNC__addId();
        }
        this.syncManager.RequestSerialization();
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        if (this.entrySystem != null)
        {
            this.entrySystem.UpdateView();
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
        if (this.syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、エントリーを行えません。: EntryManager.OnPlayerRespawn");
            return;
        }
        if (player.isLocal)
        {
            this.syncManager.ChangeOwner();
            this.UNSYNC__removeId((short)player.playerId);
            if (!this.IsEntriedAny())
            {
                this.syncManager.decided = false;
            }
            this.syncManager.RequestSerialization();
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
    private void UNSYNC__addId()
    {
        this.Ids[this.GetEmpty()] =
            (short)this.LocalPlayerId;
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
    private void UNSYNC__removeId(short id)
    {
        var ids = this.Ids;
        for (var i = ids.Length; --i >= 0; )
        {
            if (ids[i] == id)
            {
                ids[i] = 0;
            }
        }
    }
}
