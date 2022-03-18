
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>ゲームフィールドのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameField : UdonSharpBehaviour
{
    /// <value>ゲームフィールドの X 軸における部屋数。</value>
    private const int WIDTH = 8;
    /// <value>ゲームフィールドの Y 軸における部屋数。</value>
    private const int HEIGHT = 8;

    /// <value>部屋のオブジェクト一覧。</value>
    [SerializeField]
    private GameObject[] rooms = new GameObject[WIDTH * HEIGHT];

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

    /// <summary>インデックスから座標を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>X、Y座標を示す、配列。</returns>
    private int[] getXYFromIndex(int index)
    {
        return new int[] { index % WIDTH, index / WIDTH };
    }
}
