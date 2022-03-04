
using UdonSharp;
using VRC.SDKBase;

/// <summary>施錠可能エリアのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LockableArea : UdonSharpBehaviour
{
    /// <summary>ドア本体オブジェクト。</summary>
    public Door door = null;

    /// <summary>
    /// プレイヤーが当該エリアから離れた時に呼び出す、コールバック。
    /// </summary>
    /// <param name="player">対象プレイヤー オブジェクト。</param>
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal && this.door != null)
        {
            this.door.CancelLock();
        }
    }
}
