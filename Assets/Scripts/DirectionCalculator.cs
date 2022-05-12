
using UdonSharp;
using UnityEngine;

/// <summary>方向算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DirectionCalculator : UdonSharpBehaviour
{
    /// <summary>全方向の扉が閉じている状態。</summary>
    public readonly bool[] closed = new bool[] { false, false, false, false };

    /// <summary>全方向の扉が開いている状態。</summary>
    public readonly bool[] opened = new bool[] { true, true, true, true };

    /// <summary>定数一覧。</summary>
    [SerializeField]
    private Constants constants;

    /// <summary>逆方向の定数一覧。</summary>
    public int[] InvertDirection
    {
        get;
        private set;
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.constants == null)
        {
            Debug.LogError(
                "constants が null のため、初期化を行えません。: DirectionCalculator.constants");
            return;
        }
        this.InvertDirection =
            new int[]
            {
                this.constants.DIR_S,
                this.constants.DIR_W,
                this.constants.DIR_E,
                this.constants.DIR_N
            };
    }
}
