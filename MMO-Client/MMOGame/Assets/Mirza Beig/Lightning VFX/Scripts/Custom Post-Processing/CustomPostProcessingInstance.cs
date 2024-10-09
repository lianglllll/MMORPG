using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    [ExecuteAlways]
    public class CustomPostProcessingInstance : MonoBehaviour
    {
        public Material material;

        // Required for the URP renderer feature version... for some reason.

        // I can't seem to get it to work if I instance the material in the feature's renderpass and hold temp copies there.
        // I also can't use LoadControllerSettings for the same material there. Lack of documentation -> I don't really know what to do.

        // Meanwhile, BiRP is fine with using a single, shared materal that loads the properties (immediately before) each blit.

        public Material materialInstance { get; private set; }

        public delegate void CustomPostProcessingEvent(float distance, float inverseNormalizedDistance);
        public event CustomPostProcessingEvent OnUpdateMaterial;

        public float startDistance = 2.0f;
        public float length = 30.0f;

        void Start()
        {
            materialInstance = new Material(material);
        }

        void OnEnable()
        {
            if (!CustomPostProcessingLayer.instance)
            {
                return;
            }

            //print($"Add: {this}");        
            CustomPostProcessingLayer.instance.Add(this);
        }
        void OnDisable()
        {
            if (!CustomPostProcessingLayer.instance)
            {
                return;
            }

            //print($"Remove: {this}");
            CustomPostProcessingLayer.instance.Remove(this);
        }

        public void UpdateMaterial(float distance, float inverseNormalizedDistance)
        {
            OnUpdateMaterial?.Invoke(distance, inverseNormalizedDistance);
            materialInstance.CopyPropertiesFromMaterial(material);
        }

        void OnDestroy()
        {
            DestroyImmediate(materialInstance);
        }
    }
}