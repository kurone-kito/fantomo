
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームフィールド初期化の進捗状況を表示する、プログレス バーのロジック。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InitialGameProgress : UdonSharpBehaviour
{
    /// <summary><seealso cref="Image"/>コンポーネント。</summary>
    [SerializeField]
    private Image progressImage;

    /// <summary>進捗状態を設定・取得します。</summary>
    public float Progress {
        get
        {
            var img = this.progressImage;
            return img ? this.progressImage.fillAmount : 0f;
        }
        set
        {
            var img = this.progressImage;
            if (img)
            {
                img.fillAmount = value;
            }
            this.gameObject.SetActive(value < 1f);
        }
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.Progress = 0f;
    }
}
