using System;
using UdonSharp;
using UnityEngine;

/// <summary>部屋制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Room : UdonSharpBehaviour
{
    /// <summary>-X側のドアが存在するかどうか。</summary>
    [SerializeField]
    public bool existsDoorNX = false;

    /// <summary>+X側のドアが存在するかどうか。</summary>
    [SerializeField]
    public bool existsDoorPX = false;

    /// <summary>-Z側のドアが存在するかどうか。</summary>
    [SerializeField]
    public bool existsDoorNZ = false;

    /// <summary>+Z側のドアが存在するかどうか。</summary>
    [SerializeField]
    public bool existsDoorPZ = false;

    /// <summary>探索済みかどうか。</summary>
    [NonSerialized]
    public bool explored = false;

    /// <summary>地雷が存在するかどうか。</summary>
    [NonSerialized]
    public bool existsMine = false;

    /// <summary>-X側の部屋オブジェクト。</summary>
    [NonSerialized]
    public Room neighborNX = null;

    /// <summary>+X側の部屋オブジェクト。</summary>
    [NonSerialized]
    public Room neighborPX = null;

    /// <summary>-Z側の部屋オブジェクト。</summary>
    [NonSerialized]
    public Room neighborNZ = null;

    /// <summary>+Z側の部屋オブジェクト。</summary>
    [NonSerialized]
    public Room neighborPZ = null;

    /// <summary>-X側のドアと周囲のオブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject doorContainerNX = null;

    /// <summary>+X側のドアと周囲のオブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject doorContainerPX = null;

    /// <summary>-Z側のドアと周囲のオブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject doorContainerNZ = null;

    /// <summary>+Z側のドアと周囲のオブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject doorContainerPZ = null;

    /// <summary>-X側の壁オブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject wallContainerNX = null;

    /// <summary>+X側の壁オブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject wallContainerPX = null;

    /// <summary>-Z側の壁オブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject wallContainerNZ = null;

    /// <summary>+Z側の壁オブジェクトを含むコンテナ。</summary>
    [SerializeField]
    private GameObject wallContainerPZ = null;

    /// <summary>隣室一覧。</summary>
    public Room[] Neighbors
    {
        get => new Room[] { neighborNX, neighborPX, neighborNZ, neighborPZ };
        set
        {
            neighborNX = value[0];
            neighborPX = value[1];
            neighborNZ = value[2];
            neighborPZ = value[3];
        }
    }

    /// <summary>
    /// このコンポーネント初期化時に呼び出す、コールバック。
    /// </summary>
    private void Start()
    {
        UpdateVisible();
    }

    /// <summary>表示状態を更新します。</summary>
    public void UpdateVisible()
    {
        doorContainerNX.SetActive(existsDoorNX);
        doorContainerPX.SetActive(existsDoorPX);
        doorContainerNZ.SetActive(existsDoorNZ);
        doorContainerPZ.SetActive(existsDoorPZ);
        wallContainerNX.SetActive(!existsDoorNX);
        wallContainerPX.SetActive(!existsDoorPX);
        wallContainerNZ.SetActive(!existsDoorNZ);
        wallContainerPZ.SetActive(!existsDoorPZ);
    }
}
