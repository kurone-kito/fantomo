
using UdonSharp;
using UnityEngine;

/// <summary>ドア施錠・解錠スイッチの制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DoorLock : UdonSharpBehaviour
{
    /// <summary>ドア開閉スイッチを持つオブジェクト。</summary>
    public GameObject doorSwitch = null;

    /// <summary>ドア施錠状態の表示を持つオブジェクト。</summary>
    public GameObject lockedText = null;

    /// <summary>ドアが施錠しているかどうか。</summary>
    [UdonSynced]
    public bool isLocked = false;

    /// <summary>
    /// このコンポーネントをトリガーした時に呼び出す、コールバック。
    /// </summary>
    public override void Interact()
    {
        this.DisableInteractive = true;
        this.SendCustomEventDelayedSeconds(nameof(ToggleLock), 1.5f);
    }

    /// <summary>施錠状態を切り替えます。</summary>
    public void ToggleLock()
    {
        this.isLocked = !this.isLocked;
        this.DisableInteractive = false;
        var doorSwitchBehaviour = this.doorSwitch.GetComponent<DoorSwitch>();
        doorSwitchBehaviour.DisableInteractive = this.isLocked;
        if (!this.isLocked)
        {
            doorSwitchBehaviour.Interact();
        }
    }
}
