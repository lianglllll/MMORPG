using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    [ExecuteAlways]
    public class CustomPostProcessingInstanceController_RadialBlur : MonoBehaviour
    {
        CustomPostProcessingInstance fx;

        [Range(2, 128)]
        public int quality = 16;

        [Space]

        [Range(0.0f, 1.0f)]
        public float amount = 0.5f;

        [Range(0.0f, 32.0f)]
        public float power = 1.0f;

        [Space]

        public Vector2 center = new Vector2(0.5f, 0.5f);

        new Camera camera;

        void Awake()
        {
            fx = GetComponent<CustomPostProcessingInstance>();
            camera = Camera.main;
        }
        void Start()
        {

        }

        void OnEnable()
        {
            fx.OnUpdateMaterial += UpdateMaterial;
        }
        void OnDisable()
        {
            fx.OnUpdateMaterial -= UpdateMaterial;
        }

        void Update()
        {

        }

        // To simply use the material: distance = 0.0f, inverseNormalizedDistance = 1.0f.

        void UpdateMaterial(float distance, float inverseNormalizedDistance)
        {
            //print(inverseNormalizedDistance);

            Vector3 worldToScreen = camera.WorldToScreenPoint(transform.position);
            Vector3 worldToScreeNormalized = worldToScreen / new Vector2(Screen.width, Screen.height);

            center = worldToScreeNormalized;

            fx.material.SetFloat("_BlurQuality", quality);
            fx.material.SetFloat("_BlurAmount", amount * inverseNormalizedDistance);

            fx.material.SetFloat("_BlurPower", power);

            fx.material.SetFloat("_BlurCenterX", worldToScreeNormalized.x);
            fx.material.SetFloat("_BlurCenterY", worldToScreeNormalized.y);
        }
    }
}
