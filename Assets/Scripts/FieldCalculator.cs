
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
    /// <value>同期管理オブジェクト。</value>
    private SyncManager syncManager;
    /// <value>地雷設置状況一覧。</value>
    private bool[] mines;
    /// <value>探索済みのインデックス一覧。</value>
    private int[] explored;
    /// <value>
    /// <seealso cref="FieldCalculator.explored"/>に対するインデックス。
    /// </value>
    private int pointer = 0;

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
        var candidate = Random.Range(0, this.constants.NUM_ROOMS);
        var startPoint = -1;
        foreach (var index in this.getNeighborIndexes(candidate))
        {
            if (!this.mines[index])
            {
                startPoint = index;
                break;
            }
        }
        explored[pointer++] = startPoint;
        this.Calculate();

    }

    /// <summary>隣接する部屋のインデックス一覧を取得します。</summary>
    /// <param name="index">インデックス。</param>
    /// <returns>
    /// 隣接する部屋のインデックス一覧。存在しない場合、負数。
    /// </returns>
    private int[] getNeighborIndexes(int index)
    {
        var xy = this.getXYFromIndex(index);
        return new int[] {
            this.getIndexFromXY(xy[0] - 1, xy[1]),
            this.getIndexFromXY(xy[0] + 1, xy[1]),
            this.getIndexFromXY(xy[0], xy[1] - 1),
            this.getIndexFromXY(xy[0], xy[1] + 1),
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
    /// このコンポーネントが初期化された時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        if (this.managers)
        {
            this.constants = this.managers.GetComponentInChildren<Constants>();
            this.syncManager = this.managers.GetComponentInChildren<SyncManager>();
        }
        if (this.constants != null)
        {
            this.mines = new bool[this.constants.NUM_ROOMS];
            this.explored = new int[this.constants.NUM_ROOMS];
        }
    }
}
