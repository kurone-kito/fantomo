﻿
using UdonSharp;
using UnityEngine;

/// <summary>
/// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>によって生成された、
/// オブジェクト マネージャー。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ClonesManager : UdonSharpBehaviour
{
    /// <summary>管理ロジックの親となるオブジェクト。</summary>
    [SerializeField]
    private GameObject managers;

    /// <summary>生成した鍵オブジェクト一覧。</summary>
    public GameObject[] Keys
    {
        get;
        private set;
    }

    /// <summary>生成した地雷オブジェクト一覧。</summary>
    public GameObject[] Mines
    {
        get;
        private set;
    }

    /// <summary>生成した部屋オブジェクト一覧。</summary>
    public GameObject[] Rooms
    {
        get;
        private set;
    }

    /// <summary>生成したスポーン地点オブジェクト一覧。</summary>
    public GameObject[] Spawns
    {
        get;
        private set;
    }

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers == null)
        {
            Debug.LogError(
                "managers が null のため、初期化を行えません。: ClonesManager.Start");
            return;
        }
        var constants = this.managers.GetComponentInChildren<Constants>();
        this.Keys = new GameObject[constants.NUM_KEYS];
        this.Mines = new GameObject[constants.NUM_MINES];
        this.Rooms = new GameObject[constants.NUM_ROOMS];
        this.Spawns = new GameObject[constants.NUM_PLAYERS];
    }
}
