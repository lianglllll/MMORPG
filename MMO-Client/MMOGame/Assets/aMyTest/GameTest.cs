using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TP_CameraController.instance.OnStart(GameObject.Find("CtlRole").transform.Find("CameraLookTarget").transform);

    }

    // Update is called once per frame
    void Update()
    {
        Kaiyun.Event.Tick();
    }
}
