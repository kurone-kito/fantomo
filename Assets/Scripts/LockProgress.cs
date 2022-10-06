
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>施錠・解錠時のプログレス バーを操作するためのクラス。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LockProgress : UdonSharpBehaviour
{
    /// <summary>既定時のプログレス バー色。</summary>
    [SerializeField]
    private Color defaultColor;

    /// <summary>無効時のプログレス バー色。</summary>
    [SerializeField]
    private Color ignoredColor;

    /// <summary>開始時間。非稼働時は <seealso cref="float.NaN"/>。</summary>
    private float startTime = float.NaN;

    /// <summary>
    /// 終了時間。非稼働時は<seealso cref="float.MinValue"/>。
    /// </summary>
    private float endTime = float.MinValue;

    /// <summary><seealso cref="Image"/>コンポーネントを取得します。</summary>
    private Image LockImage => gameObject.GetComponent<Image>();

    /// <summary>プログレス バーを無効状態にします。</summary>
    public void IgnoreProgress()
    {
        if (LockImage != null)
        {
            LockImage.color = ignoredColor;
        }
    }

    /// <summary>プログレス バーを開始します。</summary>
    /// <param name="time">所要時間。</param>
    public void StartProgress(float time)
    {
        enabled = true;
        startTime = Time.realtimeSinceStartup;
        // 100ms 見かけ上の待機時間を減らすことにより、
        // 完了の一瞬のみ無効表示になる現象を回避している。
        endTime = Time.realtimeSinceStartup + time - 0.1f;
        if (LockImage != null)
        {
            LockImage.color = defaultColor;
        }
    }

    /// <summary>プログレス バーを強制終了します。</summary>
    public void StopProgress()
    {
        startTime = float.NaN;
        endTime = float.MinValue;
        enabled = false;
        Update();
    }

    /// <summary>進捗中かどうかを取得します。</summary>
    /// <returns>進捗中である場合、<c>true</c>。</returns>
    private bool IsProgress()
    {
        return !float.IsNaN(startTime);
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        enabled = false;
    }

    /// <summary>毎フレーム呼び出される、コールバック。</summary>
    private void Update()
    {
        var amount = float.IsNaN(startTime)
            ? 0f
            : Mathf.InverseLerp(
                startTime, endTime, Time.realtimeSinceStartup);
        if (LockImage != null)
        {
            LockImage.fillAmount = amount;
        }
        if (amount >= 1f)
        {
            StopProgress();
        }
    }
}
