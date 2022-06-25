
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EntryManager : UdonSharpBehaviour
{
    /// <value>同期管理オブジェクト。</value>
    /// <value>管理ロジックの親となるオブジェクト。</value>
    [SerializeField]
    private GameObject managers;

    /// <value>フィールド算出のロジック。</value>
    private FieldCalculator fieldCalculator;

    /// <value>同期管理オブジェクト。</value>
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
        if (this.fieldCalculator == null)
        {
            Debug.LogError(
                "fieldCalculator が null のため、エントリーを行えません。: EntryManager.Decide");
            return;
        }
        this.syncManager.ChangeOwner();
        this.syncManager.decided = true;
        this.syncManager.RequestSerialization();
        this.fieldCalculator.Calculate();
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
    public bool IsEntry()
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
    public bool IsEntryAny()
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
        if (this.IsEntry())
        {
            this.SYNC__removeId((short)this.LocalPlayerId);
        }
        else
        {
            this.SYNC__addId();
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
            this.SYNC__removeId((short)player.playerId);
            if (!this.IsEntryAny())
            {
                this.syncManager.decided = false;
            }
            this.syncManager.RequestSerialization();
        }
        if (this.entrySystem != null)
        {
            this.entrySystem.UpdateView();
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
            this.syncManager =
                this.managers.GetComponentInChildren<SyncManager>();
            this.fieldCalculator =
                this.managers.GetComponentInChildren<FieldCalculator>();
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
    private void SYNC__removeId(short id)
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
