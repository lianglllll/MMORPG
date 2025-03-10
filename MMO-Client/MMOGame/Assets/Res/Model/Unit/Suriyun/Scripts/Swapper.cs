using UnityEngine;
using System.Collections;

    public class Swapper : MonoBehaviour
    {

        public GameObject[] character;
        public int index;
        public Texture btn_tex;
        void Awake()
        {
            foreach (GameObject c in character)
            {
                c.SetActive(false);
            }
            character[0].SetActive(true);
        }
        void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 100), btn_tex))
            {
                character[index].SetActive(false);
                index++;
                index %= character.Length;
                character[index].SetActive(true);
            }
        }
    }

