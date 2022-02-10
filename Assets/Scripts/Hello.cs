
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

public class Hello : UdonSharpBehaviour
{
    public GameObject textObject = null;
    void Start()
    {
        textObject.GetComponent<Text>().text = "あのイーハトーヴォの\n透き通った風";
    }
}
