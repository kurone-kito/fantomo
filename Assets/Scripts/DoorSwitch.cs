
using UdonSharp;
using UnityEngine;

/// <summary>ドア開閉スイッチの制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorSwitch : UdonSharpBehaviour
{
    /// <value>ドア本体オブジェクト。</value>
    public GameObject door = null;

    /// <summary>
    /// このコンポーネントをトリガーした時に呼び出す、コールバック。
    /// </summary>
    public override void Interact()
    {
        this.OpenDoor();
    }

    /// <summary>
    /// ドアを開くと共に、1.5 秒後に閉じる予約をします。
    ///
    /// また同時に、ドア開閉スイッチを一時的に無効化します。
    /// </summary>
    public void OpenDoor()
    {
        var animator = door.GetComponent<Animator>();
        animator.SetBool("isOpen", true);
        this.SendCustomEventDelayedSeconds(nameof(CloseDoor), 1.5f);
        this.DisableInteractive = true;
    }

    /// <summary>
    /// ドアを閉じると共に、0.5 秒後にドア開閉スイッチを有効化します。
    /// </summary>
    public void CloseDoor()
    {
        var animator = door.GetComponent<Animator>();
        animator.SetBool("isOpen", false);
        this.SendCustomEventDelayedSeconds(nameof(EnableKnob), 0.5f);
    }

    /// <summary>ドア開閉スイッチを有効化します。</summary>
    public void EnableKnob()
    {
        this.DisableInteractive = false;
    }
}
