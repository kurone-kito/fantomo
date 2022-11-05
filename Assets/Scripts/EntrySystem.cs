using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

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

    /// <summary>プログレスバー。</summary>
    [SerializeField]
    private PrepareProgress progressBar = null;

    /// <summary>ゲーム開始ボタン本体。</summary>
    [SerializeField]
    private GameObject startButton = null;

    /// <summary>エントリーしている、プレイヤーの一覧を表示するためのラベル。</summary>
    [SerializeField]
    private Text[] playerNamesLabel = new Text[MAX_PLAYERS];

    /// <summary>エントリー管理オブジェクト。</summary>
    private EntryManager _entryManager;

    /// <summary>エントリー管理オブジェクト。</summary>
    public EntryManager EntryManagerInstance
    {
        get => _entryManager;
        set
        {
            value.EntrySystem = this;
            _entryManager = value;
            UpdateView();
        }
    }

    /// <summary>プログレスバー。</summary>
    public PrepareProgress ProgressBar => progressBar;

    /// <summary>進捗状態を設定・取得します。</summary>
    public float Progress
    {
        get => progressBar.Progress;
        set
        {
            var bar = progressBar;
            bar.gameObject.SetActive(value < 1f);
            bar.Progress = value;
            UpdateStartButtonView();
        }
    }

    /// <summary>
    /// 任意のプレイヤーがスポーンした際に呼び出すコールバック。
    /// </summary>
    public override void OnSpawn()
    {
        UpdateView();
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        UpdateView();
    }

    /// <summary>エントリーします。</summary>
    public void Entry()
    {
        if (EntryManagerInstance == null)
        {
            Debug.LogError(
                "entryManager が null のため、エントリーを行えません。: EntrySystem.Entry");
            return;
        }
        EntryManagerInstance.ToggleEntry();
        UpdateView();
    }

    /// <summary>ゲーム開始ボタンを押下した際に呼び出します。</summary>
    public void GameStart()
    {
        if (EntryManagerInstance == null)
        {
            Debug.LogError(
                "entryManager が null のため、ゲーム開始できません。: EntrySystem.GameStart");
            return;
        }
        EntryManagerInstance.Decide();
        UpdateView();
    }

    /// <summary>
    /// 自分自身がエントリーしている場合、フィールドへ転送します。
    /// </summary>
    public void TeleportToGameField()
    {
        var manager = EntryManagerInstance;
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
        gameField.TeleportToGameField();
    }

    /// <summary>ビューを最新の状態に更新します。</summary>
    public void UpdateView()
    {
        var manager = EntryManagerInstance;
        var valid = manager != null;
        var isEntry = valid && manager.IsEntry();
        var full = !isEntry && manager.GetEmpty() < 0;
        entryButtonLabel.text =
            valid && manager.Decided ? "ゲームが始まります..." :
            isEntry ? "参加を取り消す" :
            full ? "満員です" :
            "参加する";
        entryButton.interactable = !full && valid && !manager.Decided;
        UpdateStartButtonView();
        for (var i = playerNamesLabel.Length; --i >= 0;)
        {
            playerNamesLabel[i].text = GetDisplayName(manager.Ids[i]);
        }
    }

    /// <summary>ゲーム開始ボタンを最新の状態に更新します。</summary>
    private void UpdateStartButtonView()
    {
        var manager = EntryManagerInstance;
        startButton.SetActive(
            !progressBar.gameObject.activeSelf &&
            manager != null &&
            manager.IsEntry() &&
            !manager.Decided);

    }

    /// <summary>
    /// 指定した <paramref name="id"/> に対応する、
    /// プレイヤーの表示名を取得します。
    /// </summary>
    /// <param name="id">このワールドにおける、プレイヤー ID。</param>
    /// <returns>プレイヤーの表示名。無効である場合、空文字。</returns>
    private string GetDisplayName(int id)
    {
        var player = VRCPlayerApi.GetPlayerById(id);
        return player != null && player.IsValid()
            ? player.displayName
            : string.Empty;
    }
}
