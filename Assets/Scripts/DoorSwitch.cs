
using UdonSharp;
using UnityEngine;

/// <summary>ドア開閉スイッチの制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorSwitch : UdonSharpBehaviour
{
    /// <summary>ドア本体オブジェクト。</summary>
    [SerializeField]
    private Door door = null;

    /// <summary>
    /// このコンポーネントをトリガーした時に呼び出す、コールバック。
    /// </summary>
    public override void Interact()
    {
        if (this.door != null)
        {
            door.OpenDoor();
        }
    }
}
