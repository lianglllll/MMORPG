using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInputManager.Instance.Jump)
        {
            Debug.Log("1111");
        }

        if (GameInputManager.Instance.Shift) {
            GameInputManager.Instance.Change();
        }
    }
}
