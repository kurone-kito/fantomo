
using UdonSharp;
using UnityEngine;

/// <summary>
/// <para>初期化管理クラス 兼 初期化表示キャンバスのロジック。</para>
/// <para>
/// 必ず<seealso cref="UdonSharpBehaviour"/>を継承するという U# 制約のため、
/// 若干不格好ですが初期化表示キャンバスに寄生する形で実装しています。
/// </para>
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InitializeManager : UdonSharpBehaviour
{
    /// <value>プログレス バー コンポーネント。</value>
    [SerializeField]
    private InitialGameProgress progress;

    public void StartInitialize()
    {
        this.progress.Progress = 0.2f;
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.progress)
        {
            this.SendCustomEventDelayedSeconds("StartInitialize", 1f);
        }
    }
}
