
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

    /// <summary>ドア開放・施錠スイッチにおける、コンテナ。</summary>
    [SerializeField]
    private SwitchContainer switchesContainer = null;

    /// <summary>
    /// プレイヤーが当該エリアに侵入した時に呼び出す、コールバック。
    /// </summary>
    /// <param name="player">対象プレイヤー オブジェクト。</param>
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal && this.switchesContainer != null)
        {
            this.switchesContainer.enabled = true;
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
            if (this.switchesContainer != null)
            {
                this.switchesContainer.enabled = false;
            }
            if (this.door != null)
            {
                this.door.CancelLock();
            }
        }
    }
}
