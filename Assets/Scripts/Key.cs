
using UdonSharp;
using UnityEngine;

/// <summary>鍵制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Key : UdonSharpBehaviour
{
    /// <summary>取得判定。</summary>
    [SerializeField]
    private Collider body = null;

    /// <summary>吸引判定。</summary>
    [SerializeField]
    private Collider suction = null;
}
