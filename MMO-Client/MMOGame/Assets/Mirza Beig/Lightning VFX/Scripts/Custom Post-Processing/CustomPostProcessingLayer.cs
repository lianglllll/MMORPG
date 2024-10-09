using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    [ExecuteAlways]
    public class CustomPostProcessingLayer : MonoBehaviour
    {
        static public CustomPostProcessingLayer _instance;

        public static CustomPostProcessingLayer instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<CustomPostProcessingLayer>();
                }

                return _instance;
            }

            private set { }
        }

        // Events don't work well outside of play mode. Lists do.

        List<CustomPostProcessingInstance> _fxList = new List<CustomPostProcessingInstance>();
        public List<CustomPostProcessingInstance> fxList { get { return _fxList; } }

        void Awake()
        {

        }

        void Start()
        {

        }

        public void Add(CustomPostProcessingInstance fx)
        {
            fxList.Add(fx);
        }
        public void Remove(CustomPostProcessingInstance fx)
        {
            fxList.Remove(fx);
        }

        void SwapTextures(ref RenderTexture a, ref RenderTexture b)
        {
            (a, b) = (b, a);
        }

        public void GetDistanceToFX(CustomPostProcessingInstance fx, out float distance, out float normalizedInverseDistance)
        {
            distance = Vector3.Distance(transform.position, fx.transform.position);

            distance -= fx.startDistance;
            normalizedInverseDistance = Mathf.Clamp01(1.0f - (distance / fx.length));
        }

        // This is only called when attached to a camera in the BUILT-IN pipeline.
        // In URP, CustomPostProcessingRendererFeature will blit through fxList on Execute().

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (fxList.Count == 0)
            {
                Graphics.Blit(source, destination);
            }
            else
            {
                RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
                RenderTexture originalTempTexture = temp; // Need to save due to swapping, else memory leak when not released.

                for (int i = 0; i < fxList.Count; i++)
                {
                    // 1. Source and temp need to be buffered back and forth to accumulate the blits.
                    // To that end, I swap the textures, but that also means the original temp is being
                    // swapped back and forth, so saving the original helps with a simple release call later.

                    if (i > 0)
                    {
                        SwapTextures(ref temp, ref source);
                    }

                    CustomPostProcessingInstance fx = fxList[i];

                    // 2. Because I swap their values, I can keep doing a source -> temp blit.

                    // Materials can be shared, so I need to make sure immediately before the individual effect is rendered,
                    // the settings from any controllers are loaded for the material (the same material/shader re-configured per-blit).

                    GetDistanceToFX(fx, out float distance, out float normalizedInverseDistance);

                    if (normalizedInverseDistance == 0.0f)
                    {
                        Graphics.Blit(source, temp);
                    }
                    else
                    {
                        fx.UpdateMaterial(distance, normalizedInverseDistance);
                        Graphics.Blit(source, temp, fx.material);
                    }
                }

                // 3. And finally blit to destination.

                Graphics.Blit(temp, destination);
                RenderTexture.ReleaseTemporary(originalTempTexture);
            }
        }
    }
}