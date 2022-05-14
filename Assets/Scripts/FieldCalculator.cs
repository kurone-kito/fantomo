using UdonSharp;
using UnityEngine;

/// <summary>フィールド算出のロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FieldCalculator : UdonSharpBehaviour
{
    /// <value>同期管理オブジェクト。</value>
    /// <value>管理ロジックの親となるオブジェクト。</value>
    [SerializeField]
    private GameObject managers;
    /// <value>定数一覧。</value>
    private Constants constants;
    /// <value>方角周りの計算ロジック。</value>
    private DirectionCalculator directionCalculator;
    /// <value>同期管理オブジェクト。</value>
    private SyncManager syncManager;
    /// <value>方角別通路状況一覧。</value>
    private bool[][] directions;
    /// <value>地雷設置状況一覧。</value>
    private bool[] hasMines;
    /// <value>鍵設置状況一覧。</value>
    private bool[] hasKeys;
    /// <value>プレイヤー スポーン座標設置状況一覧。</value>
    private bool[] hasSpawns;

    /// <summary>
    /// フィールドの計算をします。
    /// </summary>
    public void Calculate()
    {
        if (this.constants == null)
        {
            Debug.LogError(
                "constants が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return;
        }
        if (this.syncManager == null)
        {
            Debug.LogError(
                "syncManager が null のため、フィールドを算出できません。: FieldCalculator.Calculate");
            return;
        }
    }

    /// <summary>隣接する部屋のインデックス一覧を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>
    /// 隣接する部屋のインデックス一覧。
    /// 存在しないか、通路がふさがれている場合、負数。
    /// </returns>
    private int[] getNeighborIndexes(int index)
    {
        var xy = this.getXYFromIndex(index);
        var directions = this.directions[index];
        return new int[]
        {
            directions[0] ? this.getIndexFromXY(xy[0] - 1, xy[1]) : -1,
            directions[1] ? this.getIndexFromXY(xy[0] + 1, xy[1]) : -1,
            directions[2] ? this.getIndexFromXY(xy[0], xy[1] - 1) : -1,
            directions[3] ? this.getIndexFromXY(xy[0], xy[1] + 1) : -1,
        };
    }

    /// <summary>インデックスから座標を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>X、Y座標を示す、配列。</returns>
    private int[] getXYFromIndex(int index)
    {
        var width = this.constants.ROOMS_WIDTH;
        return new int[] { index % width, index / width };
    }

    /// <summary>座標からインデックスを取得します。</summary>
    /// <param name="x">X 座標。</param>
    /// <param name="y">Y 座標。</param>
    /// <returns>インデックス。はみ出た場合、負数。</returns>
    private int getIndexFromXY(int x, int y)
    {
        var width = this.constants.ROOMS_WIDTH;
        return x < 0 || x >= width || y < 0 || y >= width
            ? -1
            : y * width + x;
    }

    /// <summary>
    /// 指定した部屋が何らかのアイテムを持っているかどうかを取得します。
    /// </summary>
    /// <param name="index">部屋の位置を示す、インデックス。</param>
    /// <returns>何らかのアイテムがある場合、<c>true</c>。</returns>
    private bool hasAnyItems(int index) =>
        this.hasMines[index] || this.hasKeys[index] || this.hasSpawns[index];

    /// <summary>
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers)
        {
            this.constants = this.managers.GetComponentInChildren<Constants>();
            this.directionCalculator = this.managers.GetComponentInChildren<DirectionCalculator>();
            this.syncManager = this.managers.GetComponentInChildren<SyncManager>();
        }
        if (this.constants != null)
        {
            this.directions = new bool[this.constants.NUM_ROOMS][];
            for (var i = this.constants.NUM_ROOMS; --i >= 0; )
            {
                this.directions[i] = new bool[] { true, true, true, true };
            }
            this.hasMines = new bool[this.constants.NUM_ROOMS];
            this.hasKeys = new bool[this.constants.NUM_ROOMS];
            this.hasSpawns = new bool[this.constants.NUM_ROOMS];
        }
    }
}
