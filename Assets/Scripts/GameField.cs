
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>ゲームフィールドのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameField : UdonSharpBehaviour
{
    /// <value>ゲーム フィールドを初期化します。</value>
    public void Initialize()
    {
    }


    /// <summary>プレイヤーをフィールドへ転送します。</summary>
    public void teleportToGameField()
    {
        var player = Networking.LocalPlayer;
        var pos = new Vector3(20, 1, 0);
        player.TeleportTo(pos, player.GetRotation());
    }
}
