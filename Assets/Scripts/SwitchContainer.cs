
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
    private float positionOffsetY;

    /// <summary>
    /// このコンポーネント初期化時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.enabled = false;
    }

    /// <summary>毎フレーム呼び出される、コールバック。</summary>
    void Update()
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
