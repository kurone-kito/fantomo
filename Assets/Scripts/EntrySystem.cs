using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

/// <summary>エントリー フォーム表示制御のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EntrySystem : UdonSharpBehaviour
{
    /// <summary>最大エントリー可能数。</summary>
    private const int MAX_PLAYERS = 3;

    /// <summary>ゲームフィールドのロジック。</summary>
    [NonSerialized]
    public GameField gameField = null;

    /// <summary>エントリーボタン本体。</summary>
    [SerializeField]
    private Button entryButton = null;

    /// <summary>エントリーボタンのラベル。</summary>
    [SerializeField]
    private Text entryButtonLabel = null;

    /// <summary>ゲーム開始ボタン本体。</summary>
    [SerializeField]
    private GameObject startButton = null;

    /// <summary>エントリーしている、プレイヤーの一覧を表示するためのラベル。</summary>
    [SerializeField]
    private Text[] playerNamesLabel = new Text[MAX_PLAYERS];

    /// <summary>エントリー管理オブジェクト。</summary>
    private EntryManager _entryManager;

    public EntryManager entryManager
    {
        get => this._entryManager;
        set
        {
            value.entrySystem = this;
            this._entryManager = value;
            this.UpdateView();
        }
    }

    /// <summary>
    /// 任意のプレイヤーがスポーンした際に呼び出すコールバック。
    /// </summary>
    public override void OnSpawn()
    {
        this.UpdateView();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.UpdateView();
    }

    /// <summary>エントリーします。</summary>
    public void Entry()
    {
        if (this.entryManager == null)
        {
            Debug.LogError(
                "entryManager が null のため、エントリーを行えません。: EntrySystem.Entry");
            return;
        }
        this.entryManager.ToggleEntry();
        this.UpdateView();
    }

    /// <summary>ゲーム開始ボタンを押下した際に呼び出します。</summary>
    public void GameStart()
    {
        if (this.entryManager == null)
        {
            Debug.LogError(
                "entryManager が null のため、ゲーム開始できません。: EntrySystem.GameStart");
            return;
        }
        this.entryManager.Decide();
        this.UpdateView();
        this.SendCustomNetworkEvent(
            NetworkEventTarget.All, nameof(teleportToGameField));
    }

    /// <summary>
    /// 自分自身がエントリーしている場合、フィールドへ転送します。
    /// </summary>
    public void teleportToGameField()
    {
        var manager = this.entryManager;
        var gameField = this.gameField;
        if (manager == null || gameField == null)
        {
            Debug.LogError(
                "entryManager または gameField が null のため、転送できません。: EntrySystem.teleportToGameField");
            return;
        }
        if (!manager.IsEntry())
        {
            Debug.LogError(
                "誰も参加していないため、転送できません。: EntrySystem.teleportToGameField");
            return;
        }
        gameField.Initialize();
        gameField.teleportToGameField();
    }

    /// <summary>ビューを最新の状態に更新します。</summary>
    public void UpdateView()
    {
        var manager = this.entryManager;
        var valid = manager != null;
        var isEntry = valid && manager.IsEntry();
        var full = !isEntry && manager.GetEmpty() < 0;
        entryButtonLabel.text =
            valid && manager.Decided ? "ゲームが始まります..." :
            isEntry ? "参加を取り消す" :
            full ? "満員です" :
            "参加する";
        entryButton.interactable = !full && valid && !manager.Decided;
        startButton.SetActive(
            isEntry && !manager.Decided && this.gameField != null);
        for (var i = playerNamesLabel.Length; --i >= 0; )
        {
            playerNamesLabel[i].text = getDisplayName(manager.Ids[i]);
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
        var manager = this.entryManager;
        if (manager == null || manager.InvalidLocalPlayer)
        {
            return id > 0 ? "Anonymous" : string.Empty;
        }
        var player = VRCPlayerApi.GetPlayerById(id);
        return player != null && player.IsValid()
            ? player.displayName
            : string.Empty;
    }
}
