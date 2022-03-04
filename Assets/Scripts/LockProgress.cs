
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>施錠・解錠時のプログレス バーを操作するためのクラス。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LockProgress : UdonSharpBehaviour
{
    /// <value>既定時のプログレス バー色。</value>
    private readonly Color defaultColor =
        new Color(0f, 0.5f, 1f, 0.5f);

    /// <value>無効時のプログレス バー色。</value>
    private readonly Color ignoredColor =
        new Color(1f, 0.2f, 0.5f);

    /// <value>開始時間。非稼働時は <seealso cref="float.NaN"/>。</value>
    private float startTime = float.NaN;

    /// <value>
    /// 終了時間。非稼働時は<seealso cref="float.MinValue"/>。
    /// </value>
    private float endTime = float.MinValue;

    /// <value><seealso cref="Image"/>コンポーネントを取得します。</value>
    private Image lockImage
    {
        get
        {
            return this.gameObject.GetComponent<Image>();
        }
    }

    /// <summary>プログレス バーを無効状態にします。</summary>
    public void IgnoreProgress()
    {
        if (this.lockImage != null)
        {
            this.lockImage.color = this.ignoredColor;
        }
    }

    /// <summary>プログレス バーを開始します。</summary>
    /// <param name="time">所要時間。</param>
    public void StartProgress(float time)
    {
        this.startTime = Time.realtimeSinceStartup;
        // 100ms 見かけ上の待機時間を減らすことにより、
        // 完了の一瞬のみ無効表示になる現象を回避している。
        this.endTime = Time.realtimeSinceStartup + time - 0.1f;
        if (this.lockImage != null)
        {
            this.lockImage.color = this.defaultColor;
        }
    }

    /// <summary>プログレス バーを強制終了します。</summary>
    public void StopProgress()
    {
        this.startTime = float.NaN;
        this.endTime = float.MinValue;
    }

    /// <summary>進捗中かどうかを取得します。</summary>
    /// <returns>進捗中である場合、<c>true</c>。</returns>
    private bool isProgress()
    {
        return !float.IsNaN(this.startTime);
    }

    /// <summary>毎フレーム呼び出される、コールバック。</summary>
    void Update()
    {
        var amount = float.IsNaN(this.startTime)
            ? 0f
            : Mathf.InverseLerp(
                this.startTime, this.endTime, Time.realtimeSinceStartup);
        if (this.lockImage != null)
        {
            this.lockImage.fillAmount = amount;
        }
        if (amount >= 1f) {
            this.StopProgress();
        }
    }
}
