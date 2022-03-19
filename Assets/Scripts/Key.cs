
using UdonSharp;
using UnityEngine;

/// <summary>鍵制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Key : UdonSharpBehaviour
{
    /// <value>取得判定。</value>
    [SerializeField]
    private Collider body = null;
    /// <value>吸引判定。</value>
    [SerializeField]
    private Collider suction = null;
}
