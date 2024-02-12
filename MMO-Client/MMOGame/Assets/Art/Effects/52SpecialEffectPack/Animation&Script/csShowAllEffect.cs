using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csShowAllEffect : MonoBehaviour
{
    public string[] EffectName;
    public Transform[] Effect;
    public Text Text1;
    public int i = 0;

    void Start()
    {
        Instantiate(Effect[i], new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update ()
    {
        Text1.text = i + 1 + ":" + EffectName[i];

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (i <= 0)
                i = 51;

            else
                i--;

           Instantiate(Effect[i], new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (i < 51)
                i++;

            else
                i = 0;

            Instantiate(Effect[i], new Vector3(0, 0, 0), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.C))
        { 
            Instantiate(Effect[i], new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
