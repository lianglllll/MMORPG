// Magica Cloth 2.
// Copyright (c) 2023 MagicaSoft.
// https://magicasoft.jp
using UnityEngine;

namespace MagicaCloth2
{
    public class TargetFPS : MonoBehaviour
    {
        public int frameRate = 60;

        void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = frameRate;
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
