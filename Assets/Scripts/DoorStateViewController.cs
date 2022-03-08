
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>ドア状態表示制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorStateViewController : UdonSharpBehaviour
{
    /// <value>解錠状態を示す定数。</value>
    public readonly int UNLOCKED = 0;

    /// <value>他プレイヤーによる施錠状態を示す定数。</value>
    public readonly int LOCKED_BY_ENEMY = 1;

    /// <value>自プレイヤーによる施錠状態を示す定数。</value>
    public readonly int LOCLED_BY_ME = 2;

    /// <value>任意のプレイヤーによる施錠状態を示す定数。</value>
    public readonly int LOCLED_BY_ANYTHING = 3;

    /// <value>自分自身による施錠における、表示文字色。</value>
    private readonly Color myLockColor =
        new Color(0f, 0.5f, 1f, 0.5f);

    /// <value>他プレイヤーによる施錠における、表示文字色。</value>
    private readonly Color enemyLockColor =
        new Color(1f, 0.2f, 0.5f);

    /// <value>ドア施錠・解錠のための、プログレス バー一覧。</value>
    public GameObject[] lockProgresses = new GameObject[2];

    /// <value>ドア施錠状態の表示一覧。</value>
    public Text[] lockedTexts = new Text[2];

    /// <value>隣接状態の表示一覧。</value>
    public Text[] neighborTexts = new Text[2];

    /// <value>プログレス バーを無効状態にします。</value>
    public void IgnoreProgress()
    {
        foreach (var progress in this.lockProgresses)
        {
            var progressController = progress.GetComponent<LockProgress>();
            if (progressController != null)
            {
                progressController.IgnoreProgress();
            }
        }
    }

    /// <summary>プログレス バーを開始します。</summary>
    /// <param name="time">所要時間。</param>
    public void StartProgress(float time)
    {
        foreach (var progress in this.lockProgresses)
        {
            var progressController = progress.GetComponent<LockProgress>();
            if (progressController != null)
            {
                progressController.StartProgress(time);
            }
        }
    }

    /// <summary>プログレス バーを停止します。</summary>
    public void StopProgress()
    {
        foreach (var progress in this.lockProgresses)
        {
            var progressController = progress.GetComponent<LockProgress>();
            if (progressController != null)
            {
                progressController.StopProgress();
            }
        }
    }

    /// <summary>施錠状態を更新します。</summary>
    /// <param name="lockState">
    /// 施錠状態。
    /// <list type="bullet">
    /// <item><c>0</c>: 解錠</item>
    /// <item><c>1</c>: 施錠</item>
    /// <item><c>2</c>: 自分自身による施錠</item>
    /// </list>
    /// </param>
    public void UpdateLockState(int lockState)
    {
        foreach (var text in this.lockedTexts)
        {
            text.gameObject.SetActive(
                (lockState & this.LOCLED_BY_ANYTHING) != 0);
            text.color = (lockState & this.LOCLED_BY_ME) != 0
                ? this.myLockColor
                : this.enemyLockColor;
        }
    }
}
