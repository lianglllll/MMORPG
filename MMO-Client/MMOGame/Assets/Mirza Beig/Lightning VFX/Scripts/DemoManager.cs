using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace MirzaBeig.LightningVFX
{
    public class DemoManager : MonoBehaviour
    {
        new Camera camera;

        public GameObject fxPrefab;
        public GameObject fxPrefab_performanceMode;

        [Space]

        public float mousePositionZ = 5.0f;

        [Space]

        // -1 = Unity uses the platform's default target frame rate.

        public int targetFrameRate = -1;

        [Space]

        public float slowMotionTimeScale = 0.4f;
        List<GameObject> spawnedLightningList = new List<GameObject>();

        [Space]

        public Light mainLight;
        float mainLightStartIntensity;

        [Space]

        public float mainLightDimIntensity = 0.35f;
        public int fullDimLightningCount = 5;

        [Space]

        public float mainLightIntensityLerpSpeed = 1.0f;

        [Space]

        public bool performanceMode;

        [Space]

        public Toggle performanceModeToggle;

        [Space]

        public GameObject postProcessVolume;
        public GameObject postProcessVolumePerformanceMode;

        void Start()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Screen.SetResolution(1920, 1080, false);
            }

            camera = Camera.main;

            mainLightStartIntensity = mainLight.intensity;

            performanceModeToggle.isOn = performanceMode;
            performanceModeToggle.onValueChanged.AddListener(OnPerformanceModeToggle);

            OnPerformanceModeToggle(performanceMode);
        }

        void Update()
        {
            Application.targetFrameRate = targetFrameRate;

            if (Input.GetKeyDown(KeyCode.F))
            {
                Time.timeScale =
                    Time.timeScale == 1.0f ? slowMotionTimeScale : 1.0f;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit raycastHitInfo, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
                {
                    GameObject fxPrefabToSpawn = !performanceModeToggle.isOn ? fxPrefab : fxPrefab_performanceMode;
                    GameObject lightning = Instantiate(fxPrefabToSpawn, raycastHitInfo.point, Quaternion.identity);

                    spawnedLightningList.Add(lightning);
                }
            }
        }

        void LateUpdate()
        {
            spawnedLightningList.RemoveAll(x => x == null);

            float normalizedLightningCount = spawnedLightningList.Count / (float)fullDimLightningCount;
            float mainLightTargetIntensity = Mathf.Lerp(mainLightStartIntensity, mainLightDimIntensity, normalizedLightningCount);

            mainLight.intensity = Mathf.Lerp(mainLight.intensity, mainLightTargetIntensity, Time.deltaTime * mainLightIntensityLerpSpeed);
        }

        void OnPerformanceModeToggle(bool value)
        {
            performanceMode = value;

            postProcessVolume.SetActive(!value);
            postProcessVolumePerformanceMode.SetActive(value);
        }
    }
}