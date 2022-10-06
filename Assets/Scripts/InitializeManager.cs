
using System;
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

    /// <summary>エントリー フォーム。</summary>
    [NonSerialized]
    public EntrySystem EntrySystem;

    /// <summary>フィールド算出のロジック。</summary>
    private FieldCalculator fieldCalculator;

    /// <summary>
    /// 動的なゲームオブジェクトの読み込みマネージャー。
    /// </summary>
    private InstantiateManager instantiateManager;

    /// <summary>同期機能マネージャー。</summary>
    private SyncManager syncManager;

    /// <summary>プログレス バーに現在の進捗状態を適用します。</summary>
    public void RefreshProgressBar()
    {
        if (EntrySystem == null)
        {
            return;
        }
        var rawFieldProgress = fieldCalculator == null
            ? 0f
            : fieldCalculator.Progress;
        var syncedFieldProgress = syncManager == null
            ? 0f
            : syncManager.fieldCalculateProgress;
        var fieldProgress = ShouldFieldCalculate()
            ? rawFieldProgress
            : syncedFieldProgress;
        var instantiateProgress = instantiateManager == null
            ? 0f
            : instantiateManager.Progress;
        EntrySystem.Progress =
            (instantiateProgress + fieldProgress) * 0.5f;
    }

    /// <summary>非同期的なバッチ初期化を開始します。</summary>
    public void StartInitializing()
    {
        if (fieldCalculator != null && ShouldFieldCalculate())
        {
            fieldCalculator.StartCalculate(this, "Dummy");
        }
        if (instantiateManager != null)
        {
            instantiateManager.StartBatchInstantiate(this, "Dummy");
        }
    }

    public void Dummy()
    {
        Debug.Log("Dummy callback");
    }

    /// <summary>
    /// ローカルプレイヤーがフィールド計算の責務を持っているかどうかを判定します。
    /// </summary>
    /// <returns>フィールド計算の責務を持っている場合、<c>true</c>。</returns>
    private bool ShouldFieldCalculate()
    {
        var local = Networking.LocalPlayer;
        return local == null || Networking.IsOwner(local, gameObject);
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        if (managers == null)
        {
            Debug.LogError(
                "managers が null のため、初期化を行えません。: InitializeManager.Start");
            return;
        }
        fieldCalculator =
            managers.GetComponentInChildren<FieldCalculator>();
        instantiateManager =
            managers.GetComponentInChildren<InstantiateManager>();
        syncManager =
            managers.GetComponentInChildren<SyncManager>();
        SendCustomEventDelayedSeconds(nameof(StartInitializing), 0f);
    }
}
