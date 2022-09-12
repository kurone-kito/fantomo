
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
        Debug.Log(string.Format("OnPlayerTriggerEnter: {0} {1}", switchesContainer == null, door == null));
        if (player.isLocal && switchesContainer != null)
        {
            switchesContainer.enabled = true;
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
            if (switchesContainer != null)
            {
                switchesContainer.enabled = false;
            }
            if (door != null)
            {
                door.CancelLock();
            }
        }
    }
}
