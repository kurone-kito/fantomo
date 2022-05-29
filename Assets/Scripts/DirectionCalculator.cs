
using UdonSharp;
using UnityEngine;

/// <summary>方向算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DirectionCalculator : UdonSharpBehaviour
{
    /// <summary>定数一覧。</summary>
    [SerializeField]
    private Constants constants;

    /// <summary>順方向の定数一覧。</summary>
    public byte[] Direction
    {
        get;
        private set;
    }

    /// <summary>逆方向の定数一覧。</summary>
    public byte[] InvertDirection
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
        this.Direction =
            new byte[]
            {
                this.constants.ROOM_FLG_DIR_N,
                this.constants.ROOM_FLG_DIR_S,
                this.constants.ROOM_FLG_DIR_W,
                this.constants.ROOM_FLG_DIR_E
            };
        this.InvertDirection =
            new byte[]
            {
                this.constants.ROOM_FLG_DIR_S,
                this.constants.ROOM_FLG_DIR_N,
                this.constants.ROOM_FLG_DIR_E,
                this.constants.ROOM_FLG_DIR_W
            };
    }
}
