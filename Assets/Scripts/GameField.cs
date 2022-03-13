
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

    /// <value>部屋のオブジェクト。</value>
    public GameObject room = null;

    /// <value>部屋のオブジェクト一覧。</value>
    private GameObject[] rooms = new GameObject[WIDTH * HEIGHT];

    /// <value>ゲーム フィールドを初期化します。</value>
    public void Initialize()
    {
        for (int i = rooms.Length; --i >= 0; )
        {
            var instanciated = VRCInstantiate(this.room);
            var xy = this.getXYFromIndex(i);
            instanciated.transform.position =
                new Vector3(xy[0] * 10, 0, xy[1] * 10) + this.transform.position;
            var roomBehaviour = instanciated.GetComponent<Room>();
            if (roomBehaviour != null)
            {
                roomBehaviour.existsDoorNX = (xy[0] > 0);
                roomBehaviour.existsDoorPX = (xy[0] < WIDTH - 1);
                roomBehaviour.existsDoorNZ = (xy[1] > 0);
                roomBehaviour.existsDoorPZ = (xy[1] < HEIGHT - 1);
            }
            this.rooms[i] = instanciated;
        }
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
