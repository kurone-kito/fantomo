
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>ドア開放・施錠スイッチにおける、コンテナのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SwitchContainer : UdonSharpBehaviour
{
    /// <summary>
    /// オブジェクトの動的調整時における、Y軸のオフセット座標。
    /// </summary>
    [SerializeField]
    private float positionOffsetY = -0.2f;

    /// <summary>
    /// このコンポーネント初期化時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        enabled = false;
    }

    /// <summary>毎フレーム呼び出される、コールバック。</summary>
    private void Update()
    {
        if (Networking.LocalPlayer == null)
        {
            return;
        }
        var trackingData =
            Networking.LocalPlayer.GetTrackingData(
                VRCPlayerApi.TrackingDataType.Head);
        transform.position = new Vector3(
            transform.position.x,
            trackingData.position.y + positionOffsetY,
            transform.position.z);
    }
}
