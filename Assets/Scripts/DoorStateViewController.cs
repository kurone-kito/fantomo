
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>ドア状態表示制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorStateViewController : UdonSharpBehaviour
{
    /// <summary>解錠状態を示す定数。</summary>
    public readonly int UNLOCKED = 0;

    /// <summary>他プレイヤーによる施錠状態を示す定数。</summary>
    public readonly int LOCKED_BY_ENEMY = 1;

    /// <summary>自プレイヤーによる施錠状態を示す定数。</summary>
    public readonly int LOCKED_BY_ME = 2;

    /// <summary>任意のプレイヤーによる施錠状態を示す定数。</summary>
    public readonly int LOCKED_BY_ANYTHING = 3;

    /// <summary>自分自身による施錠における、表示文字色。</summary>
    [SerializeField]
    private Color myLockColor;

    /// <summary>他プレイヤーによる施錠における、表示文字色。</summary>
    [SerializeField]
    private Color enemyLockColor;

    /// <summary>ドア施錠・解錠のための、プログレス バー一覧。</summary>
    [SerializeField]
    private GameObject[] lockProgresses = new GameObject[2];

    /// <summary>ドア施錠状態の表示一覧。</summary>
    [SerializeField]
    private Text[] lockedTexts = new Text[2];

    /// <summary>隣接状態の表示一覧。</summary>
    [SerializeField]
    private Text[] neighborTexts = new Text[2];

    /// <summary>プログレス バーを無効状態にします。</summary>
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
                (lockState & this.LOCKED_BY_ANYTHING) != 0);
            text.color = (lockState & this.LOCKED_BY_ME) != 0
                ? this.myLockColor
                : this.enemyLockColor;
        }
    }
}
