using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EntrySystem : UdonSharpBehaviour
{
    /// <value>最大エントリー可能数。</value>
    private const int MAX_PLAYERS = 3;

    /// <value>エントリーボタン本体。</value>
    [SerializeField]
    private Button entryButton = null;

    /// <value>エントリーボタンのラベル。</value>
    [SerializeField]
    private Text entryButtonLabel = null;

    /// <value>ゲームフィールドのロジック。</value>
    [SerializeField]
    private GameField gameField = null;

    /// <value>ゲーム開始ボタン本体。</value>
    [SerializeField]
    private GameObject startButton = null;

    /// <value>エントリーしている、プレイヤーの一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public int[] playersId = new int[MAX_PLAYERS];

    /// <value>エントリーしている、プレイヤーの一覧。</value>
    [NonSerialized]
    [UdonSynced]
    public bool gameStarted = false;

    /// <value>エントリーしている、プレイヤーの一覧を表示するためのラベル。</value>
    [SerializeField]
    private Text[] playerNamesLabel = new Text[MAX_PLAYERS];

    /// <value>現在プレイしているプレイヤーの ID を取得します。</value>
    private int localPlayerId
    {
        get =>
            Networking.LocalPlayer == null
                ? int.MaxValue
                : Networking.LocalPlayer.playerId;
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        this.updateView();
    }

    /// <summary>
    /// プレイヤーがこのワールドを去ったときに呼び出す、コールバック。
    /// </summary>
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (
            Networking.IsOwner(Networking.LocalPlayer, this.gameObject) &&
            !this.isEntriedAny())
        {
            this.gameStarted = false;
            this.RequestSerialization();
        }
        this.updateView();
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
        if (player.isLocal && !Networking.IsOwner(player, this.gameObject))
        {
            this.changeOwner();
        }
        if (Networking.IsOwner(player, this.gameObject))
        {
            this.owner__removeId(player.playerId);
            if (!this.isEntriedAny())
            {
                this.gameStarted = false;
            }
            this.RequestSerialization();
        }
        this.updateView();
    }

    /// <summary>
    /// 任意のプレイヤーがスポーンした際に呼び出すコールバック。
    /// </summary>
    public override void OnSpawn()
    {
        this.updateView();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.updateView();
    }

    /// <summary>エントリーします。</summary>
    public void Entry()
    {
        this.addOrRemoveLocalPlayer();
        this.updateView();
    }

    /// <summary>ゲーム開始ボタンを押下した際に呼び出します。</summary>
    public void GameStart()
    {
        this.changeOwner();
        this.gameStarted = true;
        this.RequestSerialization();
        this.updateView();
        this.SendCustomNetworkEvent(
            NetworkEventTarget.All, nameof(teleportToGameField));
    }

    /// <summary>
    /// 自分自身がエントリーしている場合、フィールドへ転送します。
    /// </summary>
    public void teleportToGameField()
    {
        if (this.isEntried() && this.gameField != null)
        {
            this.gameField.Initialize();
            this.gameField.teleportToGameField();
        }

    }

    /// <summary>ビューを最新の状態に更新します。</summary>
    private void updateView()
    {
        var entried = this.isEntried();
        var full = !entried && this.getEmpty() < 0;
        entryButtonLabel.text =
            this.gameStarted ? "ゲームが始まります..." :
            entried ? "参加を取り消す" :
            full ? "満員です" :
            "参加する";
        entryButton.interactable = !full && !this.gameStarted;
        startButton.SetActive(entried && !this.gameStarted);
        var nobody = this.localPlayerId == int.MaxValue;
        for (var i = this.playersId.Length; --i >= 0; )
        {
            playerNamesLabel[i].text = getDisplayName(this.playersId[i]);
        }
    }

    /// <summary>
    /// 指定した <paramref name="id"/> に対応する、
    /// プレイヤーの表示名を取得します。
    /// </summary>
    /// <param name="id">このワールドにおける、プレイヤー ID。</param>
    /// <returns>プレイヤーの表示名。無効である場合、空文字。</returns>
    private string getDisplayName(int id)
    {
        if (this.localPlayerId == int.MaxValue)
        {
            return id == int.MaxValue ? "Anonymous" : string.Empty;
        }
        var player = VRCPlayerApi.GetPlayerById(id);
        return player != null && player.IsValid()
            ? player.displayName
            : string.Empty;
    }

    /// <summary>ローカル プレイヤーを追加または削除します。</summary>
    private void addOrRemoveLocalPlayer()
    {
        this.changeOwner();
        if (this.isEntried())
        {
            this.owner__removeId(this.localPlayerId);
        }
        else
        {
            this.owner__addId();
        }
        this.RequestSerialization();
    }

    /// <summary>空きスロットのインデックスを取得します。</summary>
    /// <returns>
    /// 空きスロットのインデックス。存在しない場合、負数。
    /// </returns>
    private int getEmpty()
    {
        // NOTE: UDON では Lambda も delegate も使えない。。
        var localId = this.localPlayerId;
        if (localId == int.MaxValue)
        {
            return 0;
        }
        for (var i = this.playersId.Length; --i >= 0; )
        {
            var player = VRCPlayerApi.GetPlayerById(this.playersId[i]);
            if (player == null || !player.IsValid())
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 任意のプレイヤーがエントリーしているかどうかを取得します。
    /// </summary>
    /// <returns>エントリーしている場合、true。</returns>
    private bool isEntriedAny()
    {
        for (var i = this.playersId.Length; --i >= 0; )
        {
            var player = VRCPlayerApi.GetPlayerById(this.playersId[i]);
            if (player != null && player.IsValid())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>エントリーしているかどうかを取得します。</summary>
    /// <returns>エントリーしている場合、<c>true</c>。</returns>
    private bool isEntried()
    {
        // NOTE: UDON では Lambda も delegate も使えない。。
        var localId = this.localPlayerId;
        foreach (var id in this.playersId)
        {
            if (id == localId)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>オブジェクトオーナーを奪取・変更します。</summary>
    private void changeOwner()
    {
        var player = Networking.LocalPlayer;
        if (!Networking.IsOwner(player, this.gameObject))
        {
            Networking.SetOwner(player, this.gameObject);
        }
    }

    /// <summary>
    /// <para>プレイヤー ID をエントリー一覧に追加します。</para>
    /// <para>この関数はオブジェクトオーナーのみ使用可能です。</para>
    /// </summary>
    private void owner__addId()
    {
        this.playersId[this.getEmpty()] = this.localPlayerId;
    }

    /// <summary>
    /// <para>プレイヤー ID をエントリー一覧から削除します。</para>
    /// <para>この関数はオブジェクトオーナーのみ使用可能です。</para>
    /// </summary>
    /// <param name="id">プレイヤー ID。</param>
    private void owner__removeId(int id)
    {
        for (var i = this.playersId.Length; --i >= 0; )
        {
            if (this.playersId[i] == id)
            {
                this.playersId[i] = 0;
            }
        }
    }
}
