
using UdonSharp;
using UnityEngine;

/// <summary>壁機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Wall : UdonSharpBehaviour
{
    /// <summary>開始時に壁の位置をずらす定数。</summary>
    [SerializeField]
    private Vector3 wallMove = new Vector3(0, 3.5f, 0);

    /// <summary>
    /// <para>
    /// このコンポーネントが初期化された際に呼び出される、コールバック。
    /// </para>
    /// <para>
    /// ベイクの不整合を抑止するため、別の位置でベイクした後、
    /// 動的に座標移動をしています。
    /// </para>
    /// </summary>
    private void Start()
    {
        // これだけの処理なので、Udon graph で十分でしょうと思っていましたが、
        // transform.position に加算するだけなのに、詰みました。。
        transform.position += wallMove;
        enabled = false;
    }
}
