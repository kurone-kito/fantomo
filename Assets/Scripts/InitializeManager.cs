
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// <para>初期化処理の進捗管理クラスのロジック。</para>
/// <para>
/// 必ず<seealso cref="UdonSharpBehaviour"/>を継承するという U# 制約のため、
/// 若干不格好ですが初期化表示キャンバスに寄生する形で実装しています。
/// </para>
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InitializeManager : UdonSharpBehaviour
{
    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    /// <summary>プログレス バー コンポーネント。</summary>
    [SerializeField]
    private InitialGameProgress progress;

    /// <summary>フィールド算出のロジック。</summary>
    private FieldCalculator fieldCalculator;

    /// <summary>
    /// 動的なゲームオブジェクトの読み込みマネージャー。
    /// </summary>
    private InstantiateManager instantiateManager;

    /// <summary>プログレス バーに現在の進捗状態を適用します。</summary>
    public void RefreshProgressBar()
    {
        if (this.progress == null)
        {
            Debug.LogError(
                "progress が null のため、処理を継続できません。: InitializeManager.RefreshProgressBar");
            return;
        }
        var fieldProgress = this.shouldFieldCalculate()
            ? this.fieldCalculator.Progress
            : 1f;
        this.progress.Progress =
            (this.instantiateManager.Progress + fieldProgress) * 0.5f;
    }

    /// <summary>非同期的なバッチ初期化を開始します。</summary>
    public void StartInitializing()
    {
        if (this.fieldCalculator != null && this.shouldFieldCalculate())
        {
            this.fieldCalculator.StartCalculate(this, "Dummy");
        }
        if (this.instantiateManager != null)
        {
            this.instantiateManager.StartBatchInstantiate(this, "Dummy");
        }
    }

    public void Dummy() {
        Debug.Log("Dummy callback");
    }

    /// <summary>
    /// ローカルプレイヤーがフィールド計算の責務を持っているかどうかを判定します。
    /// </summary>
    /// <returns>フィールド計算の責務を持っている場合、<c>true</c>。</returns>
    private bool shouldFieldCalculate() {
        var local = Networking.LocalPlayer;
        return local == null || Networking.IsOwner(local, this.gameObject);
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers == null)
        {
            Debug.LogError(
                "managers が null のため、初期化を行えません。: InitializeManager.Start");
            return;
        }
        this.instantiateManager = managers.GetComponentInChildren<InstantiateManager>();
        this.fieldCalculator =
            this.managers.GetComponentInChildren<FieldCalculator>();
        this.SendCustomEventDelayedSeconds(nameof(StartInitializing), 0f);
    }
}
