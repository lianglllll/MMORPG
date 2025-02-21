using System;
using System.Collections.Generic;
using UnityEngine;


namespace TrailsFX {

    public enum TrailStyle {
        Color,
        TextureStamp,
        Clone,
        Outline,
        SpaceDistortion,
        Dash,
        Custom
    }

    public enum ColorSequence {
        Fixed,
        Cycle,
        PingPong,
        Random,
        FixedRandom
    }

    public enum PositionChangeRelative {
        World,
        OtherGameObject
    }

    public enum TrailRenderOrder {
        BeforeObject = 0,
        DrawBehind = 1,
        AlwaysOnTop = 2
    }

    public enum OutlineMethod {
        Normals,
        Rim
    }

    public static class TrailStyleProperties {

        public static bool supportsColor(this TrailStyle s) {
            return s != TrailStyle.SpaceDistortion;
        }
    }

    [ExecuteInEditMode]
    [HelpURL("https://kronnect.com/support")]
    [DefaultExecutionOrder(100)]
    public partial class TrailEffect : MonoBehaviour {

        #region Public Properties

        public TrailEffectProfile profile;
        [Tooltip("If enabled, settings will be synced with profile.")]
        public bool profileSync;
        public Transform target;

        [SerializeField]
        bool _active = true;
        public bool active { get { return _active; } set { _active = value; if (!_active) wasInactive = true; } }
        [Tooltip("By default, trails are not generated if the renderer is not visibile. This option ignores renderer visibility.")]
        public bool ignoreVisibility;
        public bool executeInEditMode;
        public int ignoreFrames;
        [Tooltip("The duration of this trail.")]
        public float duration = 0.5f;
        public bool continuous;
        [Tooltip("Use max steps to create a smooth trail if trigger condition is satisfied.")]
        public bool smooth;
        public bool checkWorldPosition;
        public float minDistance = 0.1f;
        public PositionChangeRelative worldPositionRelativeOption = PositionChangeRelative.World;
        public Transform worldPositionRelativeTransform;
        public bool checkScreenPosition = true;
        public int minPixelDistance = 10;
        public int stepsBufferSize = 1023;
        public int maxStepsPerFrame = 12;
        public bool checkTime;
        public float timeInterval = 1f;
        public bool checkCollisions;
        public bool orientToSurface = true;
        public bool ground;
        public float surfaceOffset = 0.05f;
        public LayerMask collisionLayerMask = -1;
        [Tooltip("Optional mask texture to be applied to the effect. Uses the red channel as an alpha (transparency) multiplier.")]
        public Texture2D mask;
        public TrailRenderOrder renderOrder = TrailRenderOrder.DrawBehind;
        [Tooltip("Adds additional render passes to reset stencil after trails have rendered")]
        public bool clearStencil = true;
        public UnityEngine.Rendering.CullMode cullMode = UnityEngine.Rendering.CullMode.Back;
        public int subMeshMask = -1;
        [GradientUsage(hdr: true)]
        public Gradient colorOverTime;
        public bool colorRamp;
        public Texture2D colorRampTexture;
        public Transform colorRampStart, colorRampEnd;
        public bool fadeOut = true;
        public ColorSequence colorSequence = ColorSequence.Fixed;
        [ColorUsage(showAlpha: true, hdr: true)]
        public Color color = Color.white;
        public float colorCycleDuration = 3f;
        public bool colorCycleLoop = true;
        public float pingPongSpeed = 1f;
        [GradientUsage(hdr: true)]
        public Gradient colorStartPalette;
        public Camera cam;
        public TrailStyle effect = TrailStyle.Color;
        public Material customMaterial;
        public Texture2D texture;
        public Vector3 scale = Vector3.one, scaleStartRandomMin = Vector3.one, scaleStartRandomMax = Vector3.one;
        public AnimationCurve scaleOverTime;
        [Tooltip("Ignores object scale when calculating trail scale.")]
        public bool ignoreTransformScale;
        [Tooltip("Applies an uniform scale to x/y/z axis.")]
        public bool scaleUniform;
        [Tooltip("If set, trail will be parented to this gameobject")]
        public Transform parent;
        public Vector3 localPositionRandomMin, localPositionRandomMax;
        public float laserBandWidth = 0.1f, laserIntensity = 20f, laserFlash = 0.2f;
        [ColorUsage(showAlpha: true, hdr: true)]
        public Color trailTint = new Color(0f, 0, 0.1f);

        [Tooltip("Fades out effects based on distance to camera")]
        public bool cameraDistanceFade;

        [Tooltip("The closest distance particles can get to the camera before they fade from the camera’s view.")]
        public float cameraDistanceFadeNear;

        [Tooltip("The farthest distance particles can get away from the camera before they fade from the camera’s view.")]
        public float cameraDistanceFadeFar = 1000;


        [Tooltip("Add trails only during these animation states. Optionally include start and end time, example: Attack or Attack(0.1-1.5)")]
        public string animationStates;

        [Tooltip("The animator component. If not specified, first animator component found in children or parent will be used.")]
        public Animator animator;

        public Transform lookTarget;
        public bool lookToCamera = true;
        [Range(0, 1)]
        public float textureCutOff = 0.25f;
        public OutlineMethod outlineMethod = OutlineMethod.Normals;
        public float rimPower = 2f;
        [Range(0, 1)]
        public float normalThreshold = 0.3f;

        public bool useLastAnimationState;
        public int maxBatches = 50;
        public int meshPoolSize = 256;
        [Tooltip("Interpolate vertices to provide a smoother effect.")]
        public bool interpolate;

        #endregion

        static Color colorTransparent = new Color(0, 0, 0, 0);

        const int MAX_BATCH_INSTANCES = 1023; // max number of instances submitted to GPU in a batch. This limit is defined by Unity.
        const int BAKED_GRADIENTS_LENGTH = 256; // number of baked values for the gradients

        struct SnapshotTransform {
            public Matrix4x4 matrix, parentMatrix;
            public float time;
            public int meshIndex;
            public Color color;
            public Vector3 rampStartPos;
            public Vector3 rampEndPos;
        }


        public struct SnapshotIndex {
            public float t;
            public int index;
        }

        static class ShaderParams {
            public static int ColorArray = Shader.PropertyToID("_Colors");
            public static int SubFrameKeyds = Shader.PropertyToID("_SubFrameKeys");
            public static int ColorRamp = Shader.PropertyToID("_ColorRamp");
            public static int CutOff = Shader.PropertyToID("_CutOff");
            public static int NormalThreshold = Shader.PropertyToID("_NormalThreshold");
            public static int AdditiveTint = Shader.PropertyToID("_AdditiveTint");
            public static int LaserData = Shader.PropertyToID("_LaserData");
            public static int Cull = Shader.PropertyToID("_Cull");
            public static int ZTest = Shader.PropertyToID("_ZTest");
            public static int ZOffset = Shader.PropertyToID("_ZOffset");
            public static int RampStartPositions = Shader.PropertyToID("_RampStartPos");
            public static int RampEndPositions = Shader.PropertyToID("_RampEndPos");
            public static int MaskTex = Shader.PropertyToID("_MaskTex");
            public static int ParentMatricesArray = Shader.PropertyToID("_ParentMatrices");
            public static int PivotMatrix = Shader.PropertyToID("_PivotMatrix");
            public static int RimPower = Shader.PropertyToID("_RimPower");

            public const string SKW_MASK = "TRAIL_MASK";
            public const string SKW_ALPHACLIP = "TRAIL_ALPHACLIP";
            public const string SKW_INTERPOLATE = "TRAIL_INTERPOLATE";
            public const string SKW_COLOR_RAMP = "TRAIL_COLOR_RAMP";
            public const string SKW_LOCAL = "TRAIL_LOCAL";
            public const string SKW_RIM = "TRAIL_RIM";
        }

        struct AnimationStatesInfo {
            public int hash;
            public float startTime;
            public float endTime;
        }

        SnapshotTransform[] trail;
        SnapshotIndex[] sortIndices;
        int trailIndex;
        Mesh[] meshPool;
        int meshPoolIndex;
        readonly List<Vector3> prevBakedMeshVertices = new List<Vector3>();
        float[] subFrameKeys;
        Material trailMask, trailClearMask;
        Material[] trailMaterial;
        Renderer theRenderer;
        Vector3 lastCornerMinPos, lastCornerMaxPos, lastPosition, lastRandomizedPosition, lastRelativePosition;
        Quaternion lastRotation;
        float lastIntervalTimeCheck;
        MaterialPropertyBlock properties;
        Matrix4x4[] matrices;
        Matrix4x4[] parentMatrices;
        Vector4[] colors;
        Vector4[] rampStartPositions, rampEndPositions;
        static int globalRenderQueue = 3100;
        int renderQueue;
        [NonSerialized]
        public SkinnedMeshRenderer skinnedMeshRenderer;
        [NonSerialized]
        public bool isSkinned;
        [NonSerialized]
        public ParticleSystemRenderer particleRenderer;
        [NonSerialized]
        public bool isParticle;
        int bakeTime;
        int batchNumber;
        static Mesh quadMesh;
        Dictionary<string, Material> effectMaterials;
        bool orient;
        Vector3 groundNormal;
        int startFrameCount;
        float startTime;
        float smoothDuration;
        bool isLimitedToAnimationStates;
        AnimationStatesInfo[] stateHashes;
        bool supportsGPUInstancing;
        MaterialPropertyBlock propertyBlock;
        Color colorRandomAtStart;
        Color[] bakedColorOverTime, bakedColorStartPalette;
        float[] bakedScaleOverTime;
        bool wasInactive;
        bool interpolating;
        bool usingColorRamp;
        static readonly char[] commaSeparator = new char[] { ',' };
        static readonly char[] dashSeparator = new char[] { '-' };
        bool hasParent;
        Vector3 parentPosition, lastParentPosition;
        Quaternion parentRotation, lastParentRotation;
        Vector3 rampLocalLastStart, rampLocalCurrentStart, rampLocalLastEnd, rampLocalCurrentEnd;

        void OnEnable() {
            
            CheckEditorSettings();

            // setup materials
            renderQueue = globalRenderQueue;
            globalRenderQueue += maxBatches + 2;
            if (globalRenderQueue > 3500) {
                globalRenderQueue = 3100;
            }
            if (trailMask == null) {
                trailMask = new Material(Shader.Find("TrailsFX/Mask"));
                trailMask.hideFlags = HideFlags.DontSave;
            }
            trailMask.renderQueue = renderQueue;
            if (trailClearMask == null) {
                trailClearMask = Instantiate(Resources.Load<Material>("TrailsFX/TrailClearMask"));
                trailClearMask.hideFlags = HideFlags.DontSave;
            }

            if (properties == null) {
                properties = new MaterialPropertyBlock();
            } else {
                properties.Clear();
            }
            supportsGPUInstancing = SystemInfo.supportsInstancing;
            if (!supportsGPUInstancing) {
                if (propertyBlock == null) {
                    propertyBlock = new MaterialPropertyBlock();
                } else {
                    propertyBlock.Clear();
                }
            }

            if (profileSync && profile != null) {
                profile.Load(this);
            }
            Clear();
        }

        void DestroyMaterial(Material mat) {
            if (mat != null) {
                DestroyImmediate(mat);
            }
        }

        void OnDestroy() {
            DestroyMaterial(trailMask);
            DestroyMaterial(trailClearMask);
            if (trailMaterial != null) {
                for (int k = 0; k < trailMaterial.Length; k++) {
                    DestroyMaterial(trailMaterial[k]);
                }
            }
            if (effectMaterials != null) {
                foreach (KeyValuePair<string, Material> kvp in effectMaterials) {
                    DestroyMaterial(kvp.Value);
                }
            }
            if (isSkinned && meshPool != null) {
                for (int k = 0; k < meshPool.Length; k++) {
                    if (meshPool[k] != null) {
                        DestroyImmediate(meshPool[k]);
                    }
                }
            }
        }

        void OnValidate() {
            CheckEditorSettings();
        }

        void Start() {

            startFrameCount = Time.frameCount;
            if (executeInEditMode || Application.isPlaying) {
                UpdateMaterialProperties();
            }
            colorRandomAtStart = bakedColorStartPalette[UnityEngine.Random.Range(0, BAKED_GRADIENTS_LENGTH)];
        }


#if UNITY_EDITOR
        private void OnDisable() {
            UnityEditor.EditorApplication.update -= ExecuteInEditor;
        }

        void ExecuteInEditor() {
            UnityEditor.EditorUtility.SetDirty(this);

        }
#endif


        void LateUpdate() {

            if (!executeInEditMode && !Application.isPlaying)
                return;

            if (trail == null)
                return;

            if (cam == null) {
                cam = Camera.main;
                if (cam == null) {
                    cam = FindObjectOfType<Camera>();
                    if (cam == null)
                        return;
                }
            }

            AddSnapshot();

            RenderTrail();
        }


        void OnCollisionEnter(Collision collision) {
            if (!checkCollisions || !_active)
                return;

            if (((1 << collision.gameObject.layer) & collisionLayerMask) == 0)
                return;

            Quaternion rotation;
            ContactPoint contact = collision.contacts[0];
            Vector3 pos = contact.point;
            pos += contact.normal * surfaceOffset;
            if (orientToSurface) {
                rotation = Quaternion.LookRotation(-contact.normal);
            } else {
                if (lookTarget != null) {
                    rotation = Quaternion.LookRotation(pos - lookTarget.transform.position);
                } else if (lookToCamera) {
                    Camera camera = cam;
                    if (camera == null) {
                        camera = Camera.main;
                    }
                    if (camera != null) {
                        rotation = Quaternion.LookRotation(pos - camera.transform.position);
                    } else {
                        rotation = target.rotation;
                    }
                } else {
                    rotation = target.rotation;
                }
            }
            AddSnapshot(pos, rotation);
        }



        public void CheckEditorSettings() {
            if (target == null) {
                target = transform;
            }
            if (colorOverTime == null) {
                colorOverTime = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0].color = Color.yellow;
                colorKeys[0].time = 0f;
                colorKeys[1].color = Color.yellow;
                colorKeys[1].time = 1f;
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0].alpha = 1f;
                alphaKeys[0].time = 0f;
                alphaKeys[1].alpha = 1f;
                alphaKeys[1].time = 10f;
                colorOverTime.SetKeys(colorKeys, alphaKeys);
            }
            if (scaleOverTime == null) {
                scaleOverTime = new AnimationCurve();
                Keyframe[] keys = new Keyframe[2];
                keys[0].value = 1f;
                keys[1].time = 0;
                keys[1].value = 1f;
                keys[1].time = 1;
                scaleOverTime.keys = keys;
            }
            if (colorStartPalette == null) {
                colorStartPalette = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[3];
                colorKeys[0].color = Color.red;
                colorKeys[0].time = 0f;
                colorKeys[1].color = Color.green;
                colorKeys[1].time = 0.5f;
                colorKeys[2].color = Color.blue;
                colorKeys[2].time = 1f;
                colorStartPalette.colorKeys = colorKeys;
            }
            rimPower = Mathf.Max(0.1f, rimPower);
        }

        /// <summary>
        /// Clears current trail and restarts cycle
        /// </summary>
        public void Clear() {
            UpdateMaterialProperties();
            if (theRenderer != null) {
                StoreCurrentPositions();
            }
            lastRandomizedPosition = GetRandomizedPosition();
            lastRotation = GetRotation();
            meshPoolIndex = 0;
            prevBakedMeshVertices.Clear();
            if (trail != null) {
                for (int k = 0; k < trail.Length; k++) {
                    trail[k].time = float.MinValue;
                }
            }
            trailIndex = -1;
            startFrameCount = Time.frameCount;
            startTime = Time.time;
            if (colorRampStart != null) rampLocalLastStart = target.InverseTransformPoint(colorRampStart.position);
            if (colorRampEnd != null) rampLocalLastEnd = target.InverseTransformPoint(colorRampEnd.position);
        }

        /// <summary>
        /// Restarts current trail cycle but keeps existing trail
        /// </summary>
        public void Restart() {
            startFrameCount = Time.frameCount;
            startTime = Time.time;
        }



        public void UpdateMaterialProperties() {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= ExecuteInEditor;
            if (executeInEditMode) {
                UnityEditor.EditorApplication.update += ExecuteInEditor;
            }

#endif
            CheckEditorSettings();
            if (bakedColorOverTime == null || bakedColorOverTime.Length == 0) {
                bakedColorOverTime = new Color[BAKED_GRADIENTS_LENGTH];
            }
            if (bakedScaleOverTime == null || bakedScaleOverTime.Length == 0) {
                bakedScaleOverTime = new float[BAKED_GRADIENTS_LENGTH];
            }
            if (bakedColorStartPalette == null || bakedColorStartPalette.Length == 0) {
                bakedColorStartPalette = new Color[BAKED_GRADIENTS_LENGTH];
            }
            for (int k = 0; k < BAKED_GRADIENTS_LENGTH; k++) {
                float t = (float)k / BAKED_GRADIENTS_LENGTH;
                bakedColorOverTime[k] = colorOverTime.Evaluate(t);
                bakedScaleOverTime[k] = scaleOverTime.Evaluate(t);
                bakedColorStartPalette[k] = colorStartPalette.Evaluate(t);
            }

            groundNormal = Vector3.up;
            skinnedMeshRenderer = null;
            particleRenderer = null;
            theRenderer = target.GetComponentInChildren<Renderer>();
            if (theRenderer == null) {
                trail = null;
                if (Application.isPlaying) {
                    enabled = false;
                }
                return;
            }

            isLimitedToAnimationStates = false;
            if (!string.IsNullOrEmpty(animationStates)) {
                if (animator == null) {
                    animator = target.GetComponentInChildren<Animator>();
                    if (animator == null) {
                        animator = target.GetComponentInParent<Animator>();
                    }
                }
                isLimitedToAnimationStates = animator != null;
                if (isLimitedToAnimationStates) {
                    string[] names = animationStates.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
                    int hashCount = names.Length;
                    stateHashes = new AnimationStatesInfo[hashCount];
                    for (int k = 0; k < hashCount; k++) {
                        string name = null;
                        float startTime = 0, endTime = 0;
                        string data = names[k].Trim();
                        int par0 = data.IndexOf("(");
                        int par1 = data.IndexOf(")");
                        if (par1 > par0 && par0 > 0) {
                            name = data.Substring(0, par0).Trim();
                            string interval = data.Substring(par0 + 1, par1 - par0 - 1);
                            string[] times = interval.Split(dashSeparator, StringSplitOptions.RemoveEmptyEntries);
                            if (times.Length == 2) {
                                float.TryParse(times[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out startTime);
                                float.TryParse(times[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out endTime);
                            }
                        } else {
                            name = data;
                        }
                        stateHashes[k].hash = Animator.StringToHash(name);
                        stateHashes[k].startTime = startTime;
                        stateHashes[k].endTime = endTime;
                    }
                }
            }

            isParticle = theRenderer is ParticleSystemRenderer;
            isSkinned = theRenderer is SkinnedMeshRenderer;
            if (isSkinned) {
                skinnedMeshRenderer = (SkinnedMeshRenderer)theRenderer;
                int poolSize = useLastAnimationState ? 1 : meshPoolSize;
                if (meshPool == null || meshPool.Length != poolSize) {
                    meshPool = new Mesh[meshPoolSize];
                }
                int meshPoolLength = meshPool.Length;
                for (int k = 0; k < meshPoolLength; k++) {
                    if (meshPool[k] == null) {
                        meshPool[k] = new Mesh();
                        meshPool[k].hideFlags = HideFlags.DontSave;
                    }
                }
            } else if (isParticle) {
                particleRenderer = (ParticleSystemRenderer)theRenderer;
                int poolSize = useLastAnimationState ? 1 : meshPoolSize;
                if (meshPool == null || meshPool.Length != poolSize) {
                    meshPool = new Mesh[meshPoolSize];
                }
                int meshPoolLength = meshPool.Length;
                for (int k = 0; k < meshPoolLength; k++) {
                    if (meshPool[k] == null) {
                        meshPool[k] = new Mesh();
                        meshPool[k].hideFlags = HideFlags.DontSave;
                    }
                }
            } else {
                MeshCollider mc = theRenderer.GetComponent<MeshCollider>();
                if (meshPool == null || meshPool.Length != 1) {
                    meshPool = new Mesh[1];
                }
                if (mc != null) {
                    meshPool[0] = mc.sharedMesh;
                } else {
                    MeshFilter mf = theRenderer.GetComponent<MeshFilter>();
                    if (mf != null) {
                        meshPool[0] = mf.sharedMesh;
                    }
                }
            }

            // Runtime only setup
            if (!executeInEditMode && !Application.isPlaying)
                return;


            orient = false;
            if (trailMask == null) return;
            trailMask.DisableKeyword(ShaderParams.SKW_ALPHACLIP);
            trailMask.mainTexture = null;
            trailMask.SetInt(ShaderParams.Cull, (int)cullMode);

            Material trailMat = null;
            switch (effect) {
                case TrailStyle.Color:
                    trailMat = GetEffectMaterial("TrailEffectColor");
                    break;
                case TrailStyle.TextureStamp:
                    trailMat = GetEffectMaterial("TrailEffectTextureStamp");
                    if (quadMesh == null) {
                        quadMesh = BuildQuadMesh();
                    }
                    orient = (ground && orientToSurface) || lookToCamera || lookTarget != null;
                    break;
                case TrailStyle.Clone:
                    trailMat = GetEffectMaterial("TrailEffectClone");
                    break;
                case TrailStyle.Outline:
                    trailMat = GetEffectMaterial("TrailEffectOutline");
                    break;
                case TrailStyle.SpaceDistortion:
                    trailMat = GetEffectMaterial("TrailEffectDistort");
                    break;
                case TrailStyle.Dash:
                    trailMat = GetEffectMaterial("TrailEffectLaser");
                    break;
                case TrailStyle.Custom:
                    trailMat = customMaterial;
                    break;
            }
            if (trailMat == null) {
                trail = null;
                enabled = false;
                return;
            }

            interpolating = isSkinned && interpolate && !useLastAnimationState;
            interpolating = interpolating || (isParticle && interpolate);
            usingColorRamp = colorRamp && colorRampTexture != null && colorRampStart != null && colorRampEnd != null;

            if (trailMaterial == null || trailMaterial.Length != maxBatches) {
                if (trailMaterial != null) {
                    for (int k = 0; k < trailMaterial.Length; k++) {
                        DestroyMaterial(trailMaterial[k]);
                    }
                }
                trailMaterial = new Material[maxBatches];
            }
            for (int k = 0; k < trailMaterial.Length; k++) {
                if (trailMaterial[k] == null || trailMaterial[k].shader != trailMat.shader) {
                    trailMaterial[k] = Instantiate(trailMat);
                    trailMaterial[k].hideFlags = HideFlags.DontSave;
                }
                SetMaterialProperties(trailMaterial[k]);
                trailMaterial[k].renderQueue = renderQueue + k + 1;
            }
            trailClearMask.renderQueue = renderQueue + maxBatches + 1;
            trailClearMask.SetInt(ShaderParams.Cull, (int)cullMode);

            if (trail == null || trail.Length != stepsBufferSize) {
                trail = new SnapshotTransform[stepsBufferSize];
                for (int k = 0; k < trail.Length; k++) {
                    trail[k].time = float.MinValue;
                }
                trailIndex = -1;
            }
            if (sortIndices == null || sortIndices.Length != stepsBufferSize) {
                sortIndices = new SnapshotIndex[stepsBufferSize];
            }
            if (matrices == null || matrices.Length != MAX_BATCH_INSTANCES) {
                matrices = new Matrix4x4[MAX_BATCH_INSTANCES];
            }
            if (parentMatrices == null || parentMatrices.Length != MAX_BATCH_INSTANCES) {
                parentMatrices = new Matrix4x4[MAX_BATCH_INSTANCES];
            }
            if (colors == null || colors.Length != MAX_BATCH_INSTANCES) {
                colors = new Vector4[MAX_BATCH_INSTANCES];
            }
            if (subFrameKeys == null || subFrameKeys.Length != MAX_BATCH_INSTANCES) {
                subFrameKeys = new float[MAX_BATCH_INSTANCES];
            }
            if (rampStartPositions == null || rampStartPositions.Length != MAX_BATCH_INSTANCES) {
                rampStartPositions = new Vector4[MAX_BATCH_INSTANCES];
            }
            if (rampEndPositions == null || rampEndPositions.Length != MAX_BATCH_INSTANCES) {
                rampEndPositions = new Vector4[MAX_BATCH_INSTANCES];
            }

            StoreCurrentPositions();
        }

        /// <summary>
        /// Loads and applies a different profile
        /// </summary>
        public void SetProfile(TrailEffectProfile profile) {
            if (profile != null) {
                profile.Load(this);
            }
        }

        void SetMaterialProperties(Material trailMat) {
            trailMat.SetInt(ShaderParams.Cull, (int)cullMode);
            trailMat.SetFloat(ShaderParams.ZOffset, renderOrder == TrailRenderOrder.BeforeObject ? 0.001f : 0);
            trailMat.SetInt(ShaderParams.ZTest, renderOrder == TrailRenderOrder.AlwaysOnTop ? (int)UnityEngine.Rendering.CompareFunction.Always : (int)UnityEngine.Rendering.CompareFunction.LessEqual);

            switch (effect) {
                case TrailStyle.Color:
                    trailMat.SetTexture(ShaderParams.ColorRamp, colorRampTexture);
                    break;
                case TrailStyle.TextureStamp:
                    trailMat.renderQueue = renderQueue + 1;
                    trailMat.mainTexture = texture;
                    trailMat.SetFloat(ShaderParams.CutOff, textureCutOff);
                    trailMask.mainTexture = texture;
                    trailMask.SetFloat(ShaderParams.CutOff, textureCutOff);
                    trailMask.EnableKeyword(ShaderParams.SKW_ALPHACLIP);
                    break;
                case TrailStyle.Clone:
                    Material origMat = theRenderer.sharedMaterial;
                    if (origMat != null) {
                        trailMat.mainTexture = origMat.mainTexture;
                        trailMat.mainTextureScale = origMat.mainTextureScale;
                        trailMat.mainTextureOffset = origMat.mainTextureOffset;
                        trailMat.SetFloat(ShaderParams.CutOff, textureCutOff);
                        if (textureCutOff > 0) {
                            trailMat.EnableKeyword(ShaderParams.SKW_ALPHACLIP);
                        } else {
                            trailMat.DisableKeyword(ShaderParams.SKW_ALPHACLIP);
                        }
                    }
                    break;
                case TrailStyle.Outline:
                    if (outlineMethod == OutlineMethod.Rim) {
                        trailMat.SetFloat(ShaderParams.RimPower, rimPower);
                        trailMat.EnableKeyword(ShaderParams.SKW_RIM);
                    } else {
                        trailMat.SetFloat(ShaderParams.NormalThreshold, normalThreshold);
                        trailMat.DisableKeyword(ShaderParams.SKW_RIM);
                    }
                    break;
                case TrailStyle.SpaceDistortion:
                    trailMat.SetColor(ShaderParams.AdditiveTint, trailTint);
                    break;
                case TrailStyle.Dash:
                    trailMat.SetVector(ShaderParams.LaserData, new Vector3(laserBandWidth, laserIntensity, laserFlash));
                    break;
            }
            if (mask != null) {
                trailMat.SetTexture(ShaderParams.MaskTex, mask);
                trailMat.EnableKeyword(ShaderParams.SKW_MASK);
            } else {
                trailMat.DisableKeyword(ShaderParams.SKW_MASK);
            }
            if (interpolating) {
                trailMat.EnableKeyword(ShaderParams.SKW_INTERPOLATE);
            } else {
                trailMat.DisableKeyword(ShaderParams.SKW_INTERPOLATE);
            }
            if (usingColorRamp) {
                trailMat.EnableKeyword(ShaderParams.SKW_COLOR_RAMP);
            } else {
                trailMat.DisableKeyword(ShaderParams.SKW_COLOR_RAMP);
            }
            hasParent = parent != null;
            if (hasParent) {
                trailMat.EnableKeyword(ShaderParams.SKW_LOCAL);
            } else {
                trailMat.DisableKeyword(ShaderParams.SKW_LOCAL);
            }
        }

        Material GetEffectMaterial(string materialName) {
            if (effectMaterials == null) {
                effectMaterials = new Dictionary<string, Material>();
            }
            Material mat;
            if (!effectMaterials.TryGetValue(materialName, out mat)) {
                mat = Resources.Load<Material>("TrailsFX/" + materialName);
                if (mat == null) {
                    Debug.LogError("Could not find trail material " + materialName);
                    return null;
                }
                mat = Instantiate(mat);
                mat.hideFlags = HideFlags.DontSave;
                effectMaterials[materialName] = mat;
            }
            return mat;
        }


        Mesh BuildQuadMesh() {
            Mesh mesh = new Mesh();
            mesh.name = "TrailQuadMesh";

            // Setup vertices
            Vector3[] newVertices = new Vector3[4];
            float halfHeight = 0.5f;
            float halfWidth = 0.5f;
            newVertices[0] = new Vector3(-halfWidth, -halfHeight, 0);
            newVertices[1] = new Vector3(-halfWidth, halfHeight, 0);
            newVertices[2] = new Vector3(halfWidth, -halfHeight, 0);
            newVertices[3] = new Vector3(halfWidth, halfHeight, 0);

            // Setup UVs
            Vector2[] newUVs = new Vector2[newVertices.Length];
            newUVs[0] = new Vector2(0, 0);
            newUVs[1] = new Vector2(0, 1);
            newUVs[2] = new Vector2(1, 0);
            newUVs[3] = new Vector2(1, 1);

            // Setup triangles
            int[] newTriangles = new int[] { 0, 1, 2, 3, 2, 1 };

            // Setup normals
            Vector3[] newNormals = new Vector3[newVertices.Length];
            for (int i = 0; i < newNormals.Length; i++) {
                newNormals[i] = Vector3.forward;
            }

            // Create quad
            mesh.vertices = newVertices;
            mesh.uv = newUVs;
            mesh.triangles = newTriangles;
            mesh.normals = newNormals;

            mesh.RecalculateBounds();

            return mesh;
        }


        Vector3 GetSnapshotScale() {
            Vector3 objectScale = ignoreTransformScale ? Vector3.one : target.lossyScale;
            if (scaleUniform) {
                float t = UnityEngine.Random.Range(scaleStartRandomMin.x, scaleStartRandomMax.x);
                if (isSkinned || isParticle) {
                    return new Vector3(t * scale.x, t * scale.y, t * scale.z);
                } else {
                    return new Vector3(objectScale.x * scale.x * t, objectScale.y * scale.y * t, objectScale.z * scale.z * t);
                }
            } else {
                if (isSkinned || isParticle) {
                    return new Vector3(scale.x * UnityEngine.Random.Range(scaleStartRandomMin.x, scaleStartRandomMax.x),
                        scale.y * UnityEngine.Random.Range(scaleStartRandomMin.y, scaleStartRandomMax.y),
                        scale.z * UnityEngine.Random.Range(scaleStartRandomMin.z, scaleStartRandomMax.z));
                } else {
                    return new Vector3(objectScale.x * scale.x * UnityEngine.Random.Range(scaleStartRandomMin.x, scaleStartRandomMax.x),
                        objectScale.y * scale.y * UnityEngine.Random.Range(scaleStartRandomMin.y, scaleStartRandomMax.y),
                        objectScale.z * scale.z * UnityEngine.Random.Range(scaleStartRandomMin.z, scaleStartRandomMax.z));
                }
            }
        }

        Quaternion GetRotation() {
            if (isParticle) return Quaternion.identity;
            Quaternion rot = target.rotation;
            return rot;
        }


        Vector3 GetRandomizedPosition() {
            Vector3 localPos = new Vector3(UnityEngine.Random.Range(localPositionRandomMin.x, localPositionRandomMax.x),
                                            UnityEngine.Random.Range(localPositionRandomMin.y, localPositionRandomMax.y),
                                            UnityEngine.Random.Range(localPositionRandomMin.z, localPositionRandomMax.z));
            Vector3 wpos = target.position;
            Vector3 pos;
            if (lastPosition == wpos) {
                pos = localPos + wpos;
            } else {
                pos = (Quaternion.LookRotation(wpos - lastPosition) * localPos) + wpos;
            }
            if (ground) {
                Ray ray = new Ray(target.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    pos = hit.point + pos - target.position;
                    groundNormal = hit.normal;
                }
            } else {

                if (effect == TrailStyle.TextureStamp) {
                    pos += theRenderer.bounds.center - target.position;
                }
            }

            return pos;
        }

        Color GetSnapshotColor() {
            Color snapshotColor;
            if (effect == TrailStyle.SpaceDistortion) {
                Vector2 scrPos0 = cam.WorldToViewportPoint(target.position);
                Vector2 scrPos1 = cam.WorldToViewportPoint(lastPosition);
                Vector2 diff = (scrPos0 - scrPos1).normalized;
                diff.x += 0.5f;
                diff.y += 0.5f;
                snapshotColor.r = diff.x;
                snapshotColor.g = diff.y;
                snapshotColor.b = 0;
                snapshotColor.a = 1f;
            } else {
                switch (colorSequence) {
                    case ColorSequence.Random:
                        snapshotColor = bakedColorStartPalette[UnityEngine.Random.Range(0, BAKED_GRADIENTS_LENGTH)];
                        break;
                    case ColorSequence.FixedRandom:
                        snapshotColor = colorRandomAtStart;
                        break;
                    case ColorSequence.Cycle: {
                            if (colorCycleDuration < 0) {
                                colorCycleDuration = 0.01f;
                            }
                            float t = (Time.time - startTime) / colorCycleDuration;
                            if (t > 1f && !colorCycleLoop) {
                                return colorTransparent;
                            }
                            int it = (int)((t - (int)t) * BAKED_GRADIENTS_LENGTH);
                            snapshotColor = bakedColorStartPalette[it];
                        }
                        break;
                    case ColorSequence.PingPong: {
                            float t = Mathf.PingPong((Time.time - startTime) * pingPongSpeed, 0.999f);
                            int it = (int)(t * BAKED_GRADIENTS_LENGTH);
                            snapshotColor = bakedColorStartPalette[it];
                        }
                        break;
                    default:
                        snapshotColor = color;
                        break;
                }
            }
            if (cameraDistanceFade) {
                snapshotColor.a *= ComputeCameraDistanceFade(target.position, cam.transform);
            }
            return snapshotColor;
        }

        float ComputeCameraDistanceFade(Vector3 position, Transform cameraTransform) {
            Vector3 heading = position - cameraTransform.position;
            float distance = Vector3.Dot(heading, cameraTransform.forward);
            if (distance < cameraDistanceFadeNear) {
                return 1f - Mathf.Min(1f, cameraDistanceFadeNear - distance);
            }
            if (distance > cameraDistanceFadeFar) {
                return 1f - Mathf.Min(1f, distance - cameraDistanceFadeFar);
            }
            return 1f;
        }

        void StoreCurrentPositions() {
            if (executeInEditMode || Application.isPlaying) {
                Bounds bounds = theRenderer.bounds;
                lastCornerMinPos = bounds.min;
                lastCornerMaxPos = bounds.max;
                lastPosition = target.position;
                lastRelativePosition = lastPosition;
                if (worldPositionRelativeOption == PositionChangeRelative.OtherGameObject && worldPositionRelativeTransform != null) {
                    lastRelativePosition -= worldPositionRelativeTransform.position;

                }
                lastRotation = GetRotation();
                rampLocalLastStart = rampLocalCurrentStart;
                rampLocalLastEnd = rampLocalCurrentEnd;
                if (parent != null) {
                    lastParentPosition = parent.position;
                    lastParentRotation = parent.rotation;
                }

            }
        }


        void AddSnapshot() {
            if (!_active || (!theRenderer.enabled && !ignoreVisibility)) {
                wasInactive = true;
                return;
            }

            if (wasInactive) {
                wasInactive = false;
                lastRandomizedPosition = GetRandomizedPosition();
                StoreCurrentPositions();
            }

            bool skip = Time.frameCount - startFrameCount < ignoreFrames || Time.timeScale == 0;
            if (isLimitedToAnimationStates && !skip) {
                skip = true;
                int layersCount = animator.layerCount;
                int stateHashesLength = stateHashes.Length;
                for (int l = 0; l < layersCount && skip; l++) {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(l);
                    int shortNameHash = stateInfo.shortNameHash;
                    for (int k = 0; k < stateHashesLength; k++) {
                        if (stateHashes[k].hash != shortNameHash) continue;
                        // check animation interval constraint
                        if (stateHashes[k].endTime == 0) { // no constraint
                            skip = false;
                            break;
                        }
                        float animTime = stateInfo.normalizedTime * stateInfo.length;
                        if (animTime < stateHashes[k].startTime || animTime > stateHashes[k].endTime) continue;
                        skip = false;
                        break;
                    }
                }
            }
            if (skip) {
                prevBakedMeshVertices.Clear();
                lastRandomizedPosition = GetRandomizedPosition();
                StoreCurrentPositions();
                return;
            }

            float now = Time.time;

            int steps = continuous ? maxStepsPerFrame : 0;
            if (steps == 0) {
                if (checkWorldPosition) {
                    Vector3 referencePosition = target.position;
                    Vector3 referenceLastPos = lastPosition;
                    if (worldPositionRelativeOption == PositionChangeRelative.OtherGameObject && worldPositionRelativeTransform != null) {
                        referencePosition -= worldPositionRelativeTransform.position;
                        referenceLastPos = lastRelativePosition;
                    }
                    float distance = Vector3.Distance(referencePosition, referenceLastPos);
                    if (distance >= minDistance) {
                        if (smooth) {
                            smoothDuration = now + 1f;
                        } else {
                            steps = (int)(distance / minDistance);
                        }
                    }
                }

                if (checkScreenPosition) {
                    if (minPixelDistance <= 0) {
                        minPixelDistance = 1;
                    }

                    // Difference of corners in viewport from last frame
                    Vector2 viewportPos0 = cam.WorldToViewportPoint(lastCornerMinPos);
                    Vector2 viewportPos1 = cam.WorldToViewportPoint(theRenderer.bounds.min);
                    int pixelDistance = Mathf.Max(Mathf.CeilToInt(Mathf.Abs(viewportPos1.x - viewportPos0.x) * cam.pixelWidth), Mathf.CeilToInt(Mathf.Abs(viewportPos1.y - viewportPos0.y) * cam.pixelHeight));
                    int stepsCornerMin = pixelDistance / minPixelDistance;

                    viewportPos0 = cam.WorldToViewportPoint(lastCornerMaxPos);
                    viewportPos1 = cam.WorldToViewportPoint(theRenderer.bounds.max);
                    pixelDistance = Mathf.Max((int)(Mathf.Abs(viewportPos1.x - viewportPos0.x) * cam.pixelWidth), (int)(Mathf.Abs(viewportPos1.y - viewportPos0.y) * cam.pixelHeight));
                    if (pixelDistance >= minPixelDistance) {
                        if (smooth) {
                            smoothDuration = now + 1f;
                        } else {
                            int stepsCornerMax = pixelDistance / minPixelDistance;
                            steps = Mathf.Max(steps, Mathf.Max(stepsCornerMax, stepsCornerMin));
                        }
                    }
                }

                if (checkTime) {
                    if (now - lastIntervalTimeCheck >= timeInterval) {
                        lastIntervalTimeCheck = now;
                        steps = Mathf.Max(1, steps);
                    }
                }
            }

            if (now < smoothDuration) {
                steps = maxStepsPerFrame;
            }

            if (steps <= 0)
                return;

            Color color = GetSnapshotColor();
            if (color.a == 0) return;

            if (steps > maxStepsPerFrame) {
                steps = maxStepsPerFrame;
            }

            SetupMesh(true);

            Vector3 pos = GetRandomizedPosition();
            Vector3 targetPos = Vector3.zero;
            Vector3 upwards = Vector3.up;
            if (ground && orientToSurface) {
                targetPos = pos + groundNormal;
                if (target.position != lastPosition) {
                    upwards = target.position - lastPosition;
                } else {
                    upwards = target.forward;
                }
            } else if (orient) {
                if (lookTarget != null) {
                    targetPos = lookTarget.position;
                } else {
                    Camera camera = cam;
                    if (camera == null) {
                        camera = Camera.main;
                    }
                    if (camera != null) {
                        targetPos = camera.transform.position;
                    } else {
                        orient = false;
                    }
                }
            }

            Vector3 scale = GetSnapshotScale();
            float lastFrameTime = now - Time.deltaTime;
            Quaternion rotation = GetRotation();
            bool hasParent = parent != null;
            if (hasParent) {
                parentPosition = parent.position;
                parentRotation = parent.rotation;
            }

            if (usingColorRamp) {   
                // Convert positions to local space, interpolate, then back to world space
                rampLocalCurrentStart = target.InverseTransformPoint(colorRampStart.position);
                rampLocalCurrentEnd = target.InverseTransformPoint(colorRampEnd.position);
            }


            for (int k = 0; k < steps; k++) {
                trailIndex++;
                if (trailIndex >= trail.Length) {
                    trailIndex = 0;
                }
                float t = (k + 1f) / steps;
                Vector3 p = Vector3.Lerp(lastRandomizedPosition, pos, t);

                if (orient) {
                    trail[trailIndex].matrix = Matrix4x4.TRS(p, Quaternion.LookRotation(p - targetPos, upwards), scale);
                } else {
                    Quaternion rot = Quaternion.SlerpUnclamped(lastRotation, rotation, t);
                    trail[trailIndex].matrix = Matrix4x4.TRS(p, rot, scale);
                    if (hasParent) {
                        Vector3 ppos = Vector3.Lerp(lastParentPosition, parentPosition, t);
                        Quaternion prot = Quaternion.SlerpUnclamped(lastParentRotation, parentRotation, t);
                        trail[trailIndex].parentMatrix = Matrix4x4.TRS(ppos, prot, parent.localScale).inverse;
                    }
                }

                trail[trailIndex].time = (lastFrameTime * (1f - t)) + now * t;
                trail[trailIndex].meshIndex = meshPoolIndex;
                trail[trailIndex].color = color;
                if (usingColorRamp) {
                    Vector3 localLerpedStart = Vector3.SlerpUnclamped(rampLocalLastStart, rampLocalCurrentStart, t);
                    Vector3 localLerpedEnd = Vector3.SlerpUnclamped(rampLocalLastEnd, rampLocalCurrentEnd, t);
                    trail[trailIndex].rampStartPos = localLerpedStart;
                    trail[trailIndex].rampEndPos = localLerpedEnd;
                }
            }

            lastRandomizedPosition = pos;
            StoreCurrentPositions();

        }

        void AddSnapshot(Vector3 pos, Quaternion rotation) {
            if (!_active || (!theRenderer.enabled && !ignoreVisibility))
                return;

            Color color = GetSnapshotColor();
            if (color.a == 0) return;

            SetupMesh(true);

            Vector3 scale = GetSnapshotScale();
            trailIndex++;
            if (trailIndex >= trail.Length) {
                trailIndex = 0;
            }

            trail[trailIndex].matrix = Matrix4x4.TRS(pos, rotation, scale);
            trail[trailIndex].time = Time.time;
            trail[trailIndex].meshIndex = meshPoolIndex;
            trail[trailIndex].color = color;
        }

        void RenderTrail() {
            if (duration < 0) {
                duration = 0.001f;
            }
            int count = 0;
            float now = Time.time;

            // Pick entries and compute transition 
            int sortIndicesLength = sortIndices.Length;
            for (int i = 0; i < trail.Length; i++) {
                float t = now - trail[i].time;
                if (t < duration) {
                    sortIndices[count].t = t / duration;
                    sortIndices[count].index = i;
                    count++;
                    if (count >= sortIndicesLength)
                        break;
                }
            }

            if (count == 0)
                return;

            // Sort indices
            QuickSort(0, count - 1);

            // Build batches
            batchNumber = 0;
            bool singleBatch = (useLastAnimationState || effect == TrailStyle.TextureStamp) && !isParticle;
            if (singleBatch && count <= MAX_BATCH_INSTANCES) {
                SendToGPU(meshPoolIndex, 0, count);
            } else {
                int batchMeshIndex = trail[sortIndices[0].index].meshIndex;
                int batchStartIndex = 0;
                int batchInstancesCount = 1;

                for (int k = 1; k < count; k++) {
                    int i = sortIndices[k].index;
                    int meshIndex = trail[i].meshIndex;
                    if (meshIndex != batchMeshIndex || batchInstancesCount >= MAX_BATCH_INSTANCES) {
                        // send previous batch
                        SendToGPU(batchMeshIndex, batchStartIndex, batchInstancesCount);
                        // prepare new batch
                        batchMeshIndex = meshIndex;
                        batchStartIndex += batchInstancesCount;
                        batchInstancesCount = 0;
                    }
                    batchInstancesCount++;
                }
                if (batchInstancesCount > 0) {
                    // send last batch
                    SendToGPU(batchMeshIndex, batchStartIndex, batchInstancesCount);
                }
            }
        }

        void SendToGPU(int meshIndex, int startIndex, int count) {
            if (meshIndex < 0 || meshIndex >= meshPool.Length)
                return;

            Mesh batchMesh = effect == TrailStyle.TextureStamp ? quadMesh : meshPool[meshIndex];
            if (batchMesh == null)
                return;

            int layer = target.gameObject.layer;

            if (renderOrder == TrailRenderOrder.DrawBehind && batchNumber == 0 && (theRenderer.isVisible || !ignoreVisibility)) {
                Vector3 pos = target.position;
                Vector3 sca;
                Mesh mesh;
                if (isSkinned || isParticle) {
                    mesh = SetupMesh(false);
                    sca = Vector3.one;
                } else {
                    mesh = meshPool[meshIndex];
                    sca = target.lossyScale;
                }
                if (mesh != null) {
                    Matrix4x4 m = Matrix4x4.TRS(pos, GetRotation(), sca);
                    if (subMeshMask > 0) {
                        int subMeshCount = mesh.subMeshCount;
                        for (int k = 0; k < subMeshCount; k++) {
                            if (((1 << k) & subMeshMask) != 0) {
                                Graphics.DrawMesh(mesh, m, trailMask, layer, null, k); // runs first in render queue
                                Graphics.DrawMesh(mesh, m, trailClearMask, layer, null, k); // runs last in render queue
                            }
                        }
                    } else {
                        Graphics.DrawMesh(mesh, m, trailMask, layer); // runs first in render queue
                        if (clearStencil) {
                            Graphics.DrawMesh(mesh, m, trailClearMask, layer); // runs last in render queue
                        }
                    }
                }
            }

            // Pack for instancing
            for (int o = 0; o < count; o++, startIndex++) {
                int index = sortIndices[startIndex].index;
                float t = sortIndices[startIndex].t;
                int it = (int)(BAKED_GRADIENTS_LENGTH * t) % BAKED_GRADIENTS_LENGTH;

                // Assign RGBA
                Color baseColor = trail[index].color;
                Color color = bakedColorOverTime[it];
                colors[o].x = color.r * baseColor.r;
                colors[o].y = color.g * baseColor.g;
                colors[o].z = color.b * baseColor.b;
                colors[o].w = color.a * baseColor.a;
                if (fadeOut) colors[o].w *= (1f - t);

                // Pass subframe key
                subFrameKeys[o] = (float)o / count;

                // Set matrix
                float scale = bakedScaleOverTime[it];
                matrices[o].m00 = trail[index].matrix.m00 * scale;
                matrices[o].m01 = trail[index].matrix.m01 * scale;
                matrices[o].m02 = trail[index].matrix.m02 * scale;
                matrices[o].m03 = trail[index].matrix.m03;
                matrices[o].m10 = trail[index].matrix.m10 * scale;
                matrices[o].m11 = trail[index].matrix.m11 * scale;
                matrices[o].m12 = trail[index].matrix.m12 * scale;
                matrices[o].m13 = trail[index].matrix.m13;
                matrices[o].m20 = trail[index].matrix.m20 * scale;
                matrices[o].m21 = trail[index].matrix.m21 * scale;
                matrices[o].m22 = trail[index].matrix.m22 * scale;
                matrices[o].m23 = trail[index].matrix.m23;
                matrices[o].m30 = trail[index].matrix.m30;
                matrices[o].m31 = trail[index].matrix.m31;
                matrices[o].m32 = trail[index].matrix.m32;
                matrices[o].m33 = trail[index].matrix.m33;

                // Color ramp positions
                if (usingColorRamp) {
                    rampStartPositions[o].x = trail[index].rampStartPos.x;
                    rampStartPositions[o].y = trail[index].rampStartPos.y;
                    rampStartPositions[o].z = trail[index].rampStartPos.z;
                    rampEndPositions[o].x = trail[index].rampEndPos.x;
                    rampEndPositions[o].y = trail[index].rampEndPos.y;
                    rampEndPositions[o].z = trail[index].rampEndPos.z;
                }

                if (hasParent) {
                    parentMatrices[o].m00 = trail[index].parentMatrix.m00;
                    parentMatrices[o].m01 = trail[index].parentMatrix.m01;
                    parentMatrices[o].m02 = trail[index].parentMatrix.m02;
                    parentMatrices[o].m03 = trail[index].parentMatrix.m03;
                    parentMatrices[o].m10 = trail[index].parentMatrix.m10;
                    parentMatrices[o].m11 = trail[index].parentMatrix.m11;
                    parentMatrices[o].m12 = trail[index].parentMatrix.m12;
                    parentMatrices[o].m13 = trail[index].parentMatrix.m13;
                    parentMatrices[o].m20 = trail[index].parentMatrix.m20;
                    parentMatrices[o].m21 = trail[index].parentMatrix.m21;
                    parentMatrices[o].m22 = trail[index].parentMatrix.m22;
                    parentMatrices[o].m23 = trail[index].parentMatrix.m23;
                    parentMatrices[o].m30 = trail[index].parentMatrix.m30;
                    parentMatrices[o].m31 = trail[index].parentMatrix.m31;
                    parentMatrices[o].m32 = trail[index].parentMatrix.m32;
                    parentMatrices[o].m33 = trail[index].parentMatrix.m33;
                }
            }

            // Send batch to pipeline
            properties.SetVectorArray(ShaderParams.ColorArray, colors);
            if (interpolating || usingColorRamp) {
                properties.SetFloatArray(ShaderParams.SubFrameKeyds, subFrameKeys);
            }
            if (usingColorRamp) {
                properties.SetVectorArray(ShaderParams.RampStartPositions, rampStartPositions);
                properties.SetVectorArray(ShaderParams.RampEndPositions, rampEndPositions);
            }
            if (hasParent) {
                properties.SetMatrixArray(ShaderParams.ParentMatricesArray, parentMatrices);
                properties.SetMatrix(ShaderParams.PivotMatrix, parent.localToWorldMatrix);
            }
            if (batchNumber < trailMaterial.Length - 1) {
                batchNumber++;
            } else return;

            int batchMeshSubMeshCount = batchMesh.subMeshCount;
            if (supportsGPUInstancing) {
                for (int s = 0; s < batchMeshSubMeshCount; s++) {
                    if (((1 << s) & subMeshMask) != 0) {
                        Graphics.DrawMeshInstanced(batchMesh, s, trailMaterial[batchNumber], matrices, count, properties, UnityEngine.Rendering.ShadowCastingMode.Off, false, layer);
                    }
                }
                if (clearStencil) {
                    // Clear stencil buffer
                    for (int s = 0; s < batchMeshSubMeshCount; s++) {
                        if (((1 << s) & subMeshMask) != 0) {
                            Graphics.DrawMeshInstanced(batchMesh, s, trailClearMask, matrices, count, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, layer);
                        }
                    }
                }
            } else {
                // Fallback for GPUs not supporting instancing; better than nothing :(
                for (int i = 0; i < count; i++) {
                    propertyBlock.SetVector(ShaderParams.ColorArray, colors[i]);
                    for (int s = 0; s < batchMeshSubMeshCount; s++) {
                        if (((1 << s) & subMeshMask) != 0) {
                            Graphics.DrawMesh(batchMesh, matrices[i], trailMaterial[batchNumber], layer, null, s, propertyBlock, false, false);
                        }
                    }
                }
                if (clearStencil) {
                    // Clear stencil buffer
                    for (int s = 0; s < batchMeshSubMeshCount; s++) {
                        if (((1 << s) & subMeshMask) != 0) {
                            for (int i = 0; i < count; i++) {
                                Graphics.DrawMesh(batchMesh, matrices[i], trailClearMask, layer, null, s, null, false, false);
                            }
                        }
                    }
                }
            }
        }


        Mesh SetupMesh(bool bakeMeshNow) {
            if (bakeMeshNow) {
                if (isSkinned || isParticle) {
                    int thisFrame = Time.frameCount;
                    if (thisFrame != bakeTime) {
                        bakeTime = thisFrame;
                        meshPoolIndex++;
                        if (meshPoolIndex >= meshPool.Length) {
                            meshPoolIndex = 0;
                        }
                        if (isSkinned) {
                            skinnedMeshRenderer.BakeMesh(meshPool[meshPoolIndex]);
                        } else {
#if UNITY_2022_3_OR_NEWER

                            particleRenderer.BakeMesh(meshPool[meshPoolIndex], ParticleSystemBakeMeshOptions.BakeRotationAndScale | ParticleSystemBakeMeshOptions.BakePosition);
#else
                            particleRenderer.BakeMesh(meshPool[meshPoolIndex], true);
#endif
                        }
                        if (interpolating) {
                            int prevVertexCount = prevBakedMeshVertices.Count;
                            if (prevVertexCount > 0 && meshPool[meshPoolIndex].vertexCount == prevVertexCount) {
                                meshPool[meshPoolIndex].SetUVs(1, prevBakedMeshVertices);
                                meshPool[meshPoolIndex].GetVertices(prevBakedMeshVertices);
                            } else {
                                meshPool[meshPoolIndex].GetVertices(prevBakedMeshVertices);
                                meshPool[meshPoolIndex].SetUVs(1, prevBakedMeshVertices);
                            }
                        }
                    }
                }
            }
            return meshPool[meshPoolIndex];
        }

        void QuickSort(int min, int max) {
            int i = min;
            int j = max;

            float x = sortIndices[(min + max) / 2].t;

            do {
                while (sortIndices[i].t < x) {
                    i++;
                }
                while (sortIndices[j].t > x) {
                    j--;
                }
                if (i <= j) {
                    SnapshotIndex h = sortIndices[i];
                    sortIndices[i] = sortIndices[j];
                    sortIndices[j] = h;
                    i++;
                    j--;
                }
            } while (i <= j);

            if (min < j) {
                QuickSort(min, j);
            }
            if (i < max) {
                QuickSort(i, max);
            }
        }


        /// <summary>
        /// Returns the position of a trail snapshot
        /// </summary>
        /// <param name="index">Index of the trail snapshot (0 to step buffer size defined in Trail Effect component)</param>
        /// <returns>Returns the world space position of the trail snapshot</returns>
        public Vector3 GetTrailPosition(int index) {
            return new Vector3(trail[index].matrix.m03, trail[index].matrix.m13, trail[index].matrix.m23);
        }
    }



}

