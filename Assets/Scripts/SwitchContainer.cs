
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>ドア開放・施錠スイッチにおける、コンテナのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SwitchContainer : UdonSharpBehaviour
{
    /// <summary>施錠可能エリア オブジェクト。</summary>
    [SerializeField]
    private LockableArea lockableArea = null;

    /// <summary>
    /// オブジェクトの動的調整時における、Y軸のオフセット座標。
    /// </summary>
    [SerializeField]
    private float positionOffsetY = 0.0f;

    /// <summary>毎フレーム呼び出される、コールバック。</summary>
    void Update()
    {
        if (this.lockableArea != null && this.lockableArea.isLocalPlayerExists)
        {
            var trackingData =
                Networking.LocalPlayer.GetTrackingData(
                    VRCPlayerApi.TrackingDataType.Head);
            transform.position = new Vector3(
                transform.position.x,
                trackingData.position.y + positionOffsetY,
                transform.position.z);
        }
    }
}
