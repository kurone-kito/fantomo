
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>エントリー機能のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EntryManager : UdonSharpBehaviour
{
    /// <value>同期管理オブジェクト。</value>
    [SerializeField]
    private SyncManager syncManager;

    /// <summary>エントリー状態を切り替えます。</summary>
    public void ToggleEntry()
    {
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
    }

    /// <summary>
    /// <para>
    /// 任意のプレイヤーがリスポーンした際に呼び出す、コールバック。
    /// </para>
    /// <para>
    /// このワールドでは、リスポーンはリタイアと同義であるため、
    /// エントリーを強制的に取り消しています。
    /// </para>
    /// </summary>
    /// <param name="player">リスポーンしたプレイヤー。</param>
    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
    }
}
