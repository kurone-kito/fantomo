
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームフィールド初期化の進捗状況を表示する、プログレス バーのロジック。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PrepareProgress : UdonSharpBehaviour
{
    /// <summary><seealso cref="Image"/>コンポーネント。</summary>
    [SerializeField]
    private Image progressImage;

    /// <summary>進捗状態を設定・取得します。</summary>
    public float Progress
    {
        get
        {
            var img = progressImage;
            return img ? progressImage.fillAmount : 0f;
        }
        set
        {
            var img = progressImage;
            if (img)
            {
                img.fillAmount = value;
            }
        }
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        Progress = 0f;
    }
}
