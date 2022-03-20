using UdonSharp;
using UnityEngine;

/// <summary>部屋制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Room : UdonSharpBehaviour
{
    /// <value>-X側のドアが存在するかどうか。</value>
    [SerializeField]
    public bool existsDoorNX = false;
    /// <value>+X側のドアが存在するかどうか。</value>
    [SerializeField]
    public bool existsDoorPX = false;
    /// <value>-Z側のドアが存在するかどうか。</value>
    [SerializeField]
    public bool existsDoorNZ = false;
    /// <value>+Z側のドアが存在するかどうか。</value>
    [SerializeField]
    public bool existsDoorPZ = false;
    /// <value>-X側のドアと周囲のオブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject doorContainerNX = null;
    /// <value>+X側のドアと周囲のオブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject doorContainerPX = null;
    /// <value>-Z側のドアと周囲のオブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject doorContainerNZ = null;
    /// <value>+Z側のドアと周囲のオブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject doorContainerPZ = null;
    /// <value>-X側の壁オブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject wallContainerNX = null;
    /// <value>+X側の壁オブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject wallContainerPX = null;
    /// <value>-Z側の壁オブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject wallContainerNZ = null;
    /// <value>+Z側の壁オブジェクトを含むコンテナ。</value>
    [SerializeField]
    private GameObject wallContainerPZ = null;
    /// <summary>
    /// このコンポーネント初期化時に呼び出す、コールバック。
    /// </summary>
    void Start()
    {
        this.UpdateVisible();
    }

    /// <summary>表示状態を更新します。</summary>
    public void UpdateVisible()
    {
        this.doorContainerNX.SetActive(this.existsDoorNX);
        this.doorContainerPX.SetActive(this.existsDoorPX);
        this.doorContainerNZ.SetActive(this.existsDoorNZ);
        this.doorContainerPZ.SetActive(this.existsDoorPZ);
        this.wallContainerNX.SetActive(!this.existsDoorNX);
        this.wallContainerPX.SetActive(!this.existsDoorPX);
        this.wallContainerNZ.SetActive(!this.existsDoorNZ);
        this.wallContainerPZ.SetActive(!this.existsDoorPZ);
    }
}
