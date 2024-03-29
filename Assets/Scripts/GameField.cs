﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>ゲームフィールドのロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameField : UdonSharpBehaviour
{
    /// <summary>ゲームフィールドの X 軸における部屋数。</summary>
    private const int WIDTH = 8;

    /// <summary>ゲームフィールドの Y 軸における部屋数。</summary>
    private const int HEIGHT = 8;

    /// <summary>地雷の設置数。</summary>
    private const int MINES = 9;

    /// <summary>部屋のオブジェクト一覧。</summary>
    [NonSerialized]
    public GameObject[] rooms = new GameObject[WIDTH * HEIGHT];

    /// <summary>ゲーム フィールドを初期化します。</summary>
    public void Initialize()
    {
        // InitializeRooms();
        // PlaceMines();
    }

    /// <summary>プレイヤーをフィールドへ転送します。</summary>
    public void TeleportToGameField()
    {
        var player = Networking.LocalPlayer;
        if (player == null)
        {
            Debug.LogError("UnityEditor ではここから先には進めません。");
            return;
        }
        player.TeleportTo(new Vector3(19, 1, 5), player.GetRotation());
    }

    /// <summary>地雷の配置候補先を決定します。</summary>
    private void PlaceMines()
    {
        var candidate = (int)(UnityEngine.Random.value * rooms.Length);
        var roomScript = rooms[candidate].GetComponent<Room>();
        roomScript.existsMine = true;

        // TODO: すでに地雷が配置されている部屋は、抽選をやり直す。
        // TODO: 四方向のうち、少なくとも一方向に地雷がない部屋がない場合は、抽選をやり直す。
        // TODO: 一部屋でも探索不能な部屋が発生する場合は、抽選をやり直す。
    }

    /// <summary>各部屋を初期化します。</summary>
    private void InitializeRooms()
    {
        for (var i = rooms.Length; --i >= 0;)
        {
            PlaceWall(i);
            SetNeighbors(i);
        }
    }

    /// <summary>壁を配置します。</summary>
    /// <param name="index">部屋のインデックス。</param>
    private void PlaceWall(int index)
    {
        var room = rooms[index];
        var xy = GetXYFromIndex(index);
        var roomScript = room.GetComponent<Room>();
        roomScript.existsDoorNX = xy[0] > 0;
        roomScript.existsDoorPX = xy[0] < WIDTH - 1;
        roomScript.existsDoorNZ = xy[1] > 0;
        roomScript.existsDoorPZ = xy[1] < HEIGHT - 1;
        roomScript.UpdateVisible();
    }

    /// <summary>隣室を設定します。</summary>
    /// <param name="index">部屋のインデックス。</param>
    private void SetNeighbors(int index)
    {
        var room = rooms[index];
        var neighborsIndex = GetNeighborIndexes(index);
        var neighbors = new Room[neighborsIndex.Length];
        for (var j = neighborsIndex.Length; --j >= 0;)
        {
            var i = neighborsIndex[j];
            neighbors[j] =
                i < 0 ? null : rooms[i].GetComponent<Room>();
        }
        var roomScript = room.GetComponent<Room>();
        roomScript.Neighbors = neighbors;
    }

    /// <summary>探索フラグを初期化します。</summary>
    /// <returns>探索を開始する際の起点となる、インデックス。</returns>
    private int ClearExploringFlag()
    {
        var result = -1;
        for (var i = rooms.Length; --i >= 0;)
        {
            var roomScript = rooms[i].GetComponent<Room>();
            roomScript.explored = false;
            if (result < 0 && !roomScript.existsMine)
            {
                result = i;
                roomScript.explored = true;
            }
        }
        return result;
    }

    /// <summary>隣接する部屋のインデックス一覧を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>
    /// 隣接する部屋のインデックス一覧。存在しない場合、負数。
    /// </returns>
    private int[] GetNeighborIndexes(int index)
    {
        var xy = GetXYFromIndex(index);
        return new int[] {
            GetIndexFromXY(xy[0] - 1, xy[1]),
            GetIndexFromXY(xy[0] + 1, xy[1]),
            GetIndexFromXY(xy[0], xy[1] - 1),
            GetIndexFromXY(xy[0], xy[1] + 1),
        };
    }

    /// <summary>インデックスから座標を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>X、Y座標を示す、配列。</returns>
    private int[] GetXYFromIndex(int index)
    {
        return new int[] { index % WIDTH, index / WIDTH };
    }

    /// <summary>座標からインデックスを取得します。</summary>
    /// <param name="x">X 座標。</param>
    /// <param name="y">Y 座標。</param>
    /// <returns>インデックス。はみ出た場合、負数。</returns>
    private int GetIndexFromXY(int x, int y)
    {
        return x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT
            ? -1
            : (y * WIDTH) + x;
    }
}
