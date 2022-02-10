using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EntrySystem : UdonSharpBehaviour
{
    /// <summary>最大エントリー可能数。</summary>
    private const int MAX_PLAYERS = 3;

    /// <summary>エントリーボタン本体。</summary>
    public GameObject entryButton = null;

    /// <summary>エントリーボタンのラベル。</summary>
    public GameObject entryButtonLabel = null;

    /// <summary>ゲーム開始ボタン本体。</summary>
    public GameObject startButton = null;

    /// <summary>エントリーしている、プレイヤーの一覧。</summary>
    [UdonSynced]
    public int[] playersId = new int[MAX_PLAYERS];

    /// <summary>エントリーしている、プレイヤーの一覧。</summary>
    [UdonSynced]
    public bool gameStarted = false;

    /// <summary>エントリーしている、プレイヤーの一覧を表示するためのラベル。</summary>
    public GameObject[] playerNamesLabel = new GameObject[MAX_PLAYERS];

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
        this.updateView();
    }

    /// <summary>
    /// 任意のプレイヤーがリスポーンした際に呼び出すコールバック。
    /// このワールドでは、リスポーンはリタイアと同義であるため、
    /// エントリーを強制的に取り消しています。
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
            this.RequestSerialization();
        }
        this.updateView();
    }

    /// <summary>
    /// 任意のプレイヤーがこのワールドにスポーンした際に呼び出すコールバック。
    /// </summary>
    public override void OnSpawn()
    {
        this.updateView();
    }

    /// <summary>このワールド初期化時に呼び出す、コールバック。</summary>
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

    public void GameStart()
    {
        this.changeOwner();
        this.gameStarted = true;
        this.RequestSerialization();
        this.updateView();
        this.teleportEntriedPlayers();
    }

    /// <summary>
    /// エントリーしているプレイヤーをフィールドへ転送します。
    /// </summary>
    private void teleportEntriedPlayers()
    {
        foreach (var id in this.playersId)
        {
            var player = VRCPlayerApi.GetPlayerById(id);
            if (player != null && player.IsValid())
            {
                player.TeleportTo(
                    new Vector3(20, 1, 0),
                    player.GetRotation());
            }
        }
    }

    private void updateView()
    {
        var entried = this.isEntried();
        var full = !entried && this.getEmpty() < 0;
        entryButtonLabel.GetComponent<Text>().text =
            this.gameStarted ? "ゲームが始まります..." :
            entried ? "参加を取り消す" :
            full ? "満員です" :
            "参加する";
        entryButton.GetComponent<Button>().interactable =
            !full && !this.gameStarted;
        startButton.SetActive(entried && !this.gameStarted);
        for (var i = this.playersId.Length; --i >= 0; )
        {
            var id = this.playersId[i];
            var player = VRCPlayerApi.GetPlayerById(id);
            playerNamesLabel[i].GetComponent<Text>().text =
                player != null && player.IsValid()
                    ? string.Format("{0}(ID: {1})", player.displayName, id)
                    : string.Empty;
        }
    }

    /// <summary>ローカル プレイヤーを追加または削除します。</summary>
    private void addOrRemoveLocalPlayer()
    {
        this.changeOwner();
        if (this.isEntried())
        {
            this.owner__removeId(Networking.LocalPlayer.playerId);
        }
        else
        {
            this.owner__addId();
        }
        this.RequestSerialization();
    }

    /// <summary>空きスロットのインデックスを取得します。</summary>
    /// <returns>空きスロットのインデックス。存在しない場合、負数。</returns>
    private int getEmpty()
    {
        // NOTE: UDON では Lambda も delegate も使えない。。
        var localId = Networking.LocalPlayer.playerId;
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

    /// <summary>エントリーしているかどうかを取得します。</summary>
    /// <returns>エントリーしている場合、true。</returns>
    private bool isEntried()
    {
        // NOTE: UDON では Lambda も delegate も使えない。。
        var localId = Networking.LocalPlayer.playerId;
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
    /// プレイヤー ID をエントリー一覧に追加します。
    /// この関数はオブジェクトオーナーのみ使用可能です。
    /// </summary>
    private void owner__addId()
    {
        this.playersId[this.getEmpty()] = Networking.LocalPlayer.playerId;
    }

    /// <summary>
    /// プレイヤー ID をエントリー一覧から削除します。
    /// この関数はオブジェクトオーナーのみ使用可能です。
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
