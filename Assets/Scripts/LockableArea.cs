
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>施錠可能エリアのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LockableArea : UdonSharpBehaviour
{
    /// <summary>ドア本体オブジェクト。</summary>
    [SerializeField]
    private Door door = null;

    /// <summary>プレイヤーが当該エリアに存在するかどうか。</summary>
    public bool isLocalPlayerExists
    {
        get;
        private set;
    }

    /// <summary>
    /// プレイヤーが当該エリアに侵入した時に呼び出す、コールバック。
    /// </summary>
    /// <param name="player">対象プレイヤー オブジェクト。</param>
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            this.isLocalPlayerExists = true;
        }
    }

    /// <summary>
    /// プレイヤーが当該エリアから離れた時に呼び出す、コールバック。
    /// </summary>
    /// <param name="player">対象プレイヤー オブジェクト。</param>
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            this.isLocalPlayerExists = false;
            if (this.door != null)
            {
                this.door.CancelLock();
            }
        }
    }

    /// <summary>
    /// このコンポーネント初期化時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.isLocalPlayerExists = false;
    }
}
