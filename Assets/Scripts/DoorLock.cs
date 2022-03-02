
using UdonSharp;

/// <summary>ドア施錠・解錠スイッチの制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DoorLock : UdonSharpBehaviour
{
    /// <value>ドア本体オブジェクト。</value>
    public Door door = null;

    /// <summary>
    /// このコンポーネントをトリガーした時に呼び出す、コールバック。
    /// </summary>
    public override void Interact()
    {
        if (this.door != null)
        {
            door.ToggleLock();
        }
    }
}
