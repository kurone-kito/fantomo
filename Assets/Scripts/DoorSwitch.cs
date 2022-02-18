
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoorSwitch : UdonSharpBehaviour
{
    public GameObject door = null;

    public override void Interact()
    {
        var animator = door.GetComponent<Animator>();
        animator.SetBool("isOpen", !animator.GetBool("isOpen"));
    }
}
