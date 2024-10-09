using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    // BUILT-IN PIPELINE ONLY. Attach to a camera.
    // This is used to immediately apply some custom post-processing effect after Unity's post-processing.

    [ExecuteAlways]
    public class CustomPostProcessing : MonoBehaviour
    {
        public Material material;

        void Start()
        {

        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
        }
    }
}