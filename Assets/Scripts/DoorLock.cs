
using UdonSharp;
using UnityEngine;
using VRC.Udon;

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
        this.UpdateInteract();
        var doorSwitchBehaviour = this.doorSwitch.GetComponent<DoorSwitch>();
        doorSwitchBehaviour.DisableInteractive = this.isLocked;
        if (!this.isLocked)
        {
            doorSwitchBehaviour.Interact();
        }
    }

    /// <summary>表示状態を更新します。</summary>
    private void UpdateInteract()
    {
        // ! ここだけ Generics 使うと怒られる。なんでやねん。
        var selfBehaviour =
            (UdonBehaviour)this.GetComponent(typeof(UdonBehaviour));
        selfBehaviour.InteractionText =
            this.isLocked ? "解錠して開く" : "施錠";
        this.lockedText.SetActive(this.isLocked);
    }
}
