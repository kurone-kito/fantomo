
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
    private void Start()
    {
        if (constants == null)
        {
            Debug.LogError(
                "constants が null のため、初期化を行えません。: DirectionCalculator.constants");
            return;
        }
        Direction =
            new byte[]
            {
                (byte)ROOM_FLG.DIR_N,
                (byte)ROOM_FLG.DIR_S,
                (byte)ROOM_FLG.DIR_W,
                (byte)ROOM_FLG.DIR_E,
            };
        InvertDirection =
            new byte[]
            {
                (byte)ROOM_FLG.DIR_S,
                (byte)ROOM_FLG.DIR_N,
                (byte)ROOM_FLG.DIR_E,
                (byte)ROOM_FLG.DIR_W,
            };
    }
}
