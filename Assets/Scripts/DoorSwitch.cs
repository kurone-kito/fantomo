
using UdonSharp;
using UnityEngine;

public class DoorSwitch : UdonSharpBehaviour
{
    public GameObject door = null;
    void Start()
    {

    }

    public override void Interact()
    {
        Debug.Log("Interact");
        // door.GetComponent<Animator>().enabled = !door.GetComponent<Animator>().enabled;
    }
}
