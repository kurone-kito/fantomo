
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
    /// <value>地雷の設置数。</value>
    private const int MINES = 9;

    /// <value>部屋のオブジェクト一覧。</value>
    [SerializeField]
    private GameObject[] rooms = new GameObject[WIDTH * HEIGHT];

    /// <value>ゲーム フィールドを初期化します。</value>
    public void Initialize()
    {
        this.placeWall();
        this.placeMines();
    }

    /// <summary>プレイヤーをフィールドへ転送します。</summary>
    public void teleportToGameField()
    {
        var player = Networking.LocalPlayer;
        var pos = new Vector3(20, 1, 0);
        player.TeleportTo(pos, player.GetRotation());
    }

    /// <summary>地雷の配置候補先を決定します。</summary>
    private void placeMines()
    {
        var minesIndex = new int[MINES];
        for (int i = MINES; --i >= 0;)
        {
            var candidate = (int)(Random.value * this.rooms.Length);
            // TODO: すでに地雷が配置されている部屋は、抽選をやり直す。
            // TODO: 四方向のうち、少なくとも一方向に地雷がない部屋がない場合は、抽選をやり直す。
            // TODO: 一部屋でも探索不能な部屋が発生する場合は、抽選をやり直す。
        }
    }

    /// <value>壁を配置します。</value>
    public void placeWall()
    {
        for (var i = rooms.Length; --i >= 0; )
        {
            var room = rooms[i];
            var xy = this.getXYFromIndex(i);
            var roomScript = room.GetComponent<Room>();
            roomScript.existsDoorNX = xy[0] > 0;
            roomScript.existsDoorPX = xy[0] < WIDTH - 1;
            roomScript.existsDoorNZ = xy[1] > 0;
            roomScript.existsDoorPZ = xy[1] < HEIGHT - 1;
            roomScript.UpdateVisible();
        }
    }

    /// <summary>インデックスから座標を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>X、Y座標を示す、配列。</returns>
    private int[] getXYFromIndex(int index)
    {
        return new int[] { index % WIDTH, index / WIDTH };
    }
}
