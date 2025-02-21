using UnityEngine;

namespace TrailsFX {
    [CreateAssetMenu (menuName = "Trail FX Profile", fileName = "Trail FX Profile", order = 100)]
	public partial class TrailEffectProfile : ScriptableObject
	{
		public TrailEffectProfile profile;
		[Tooltip ("If enabled, settings will be synced with profile.")]
		public bool active = true;
		[Tooltip("By default, trails are not generated if the renderer is not visibile. This option ignores renderer visibility.")]
		public bool ignoreVisibility;
		public int ignoreFrames = 3;
		public float duration = 0.5f;
		public bool continuous;
		public bool smooth;
		public bool checkWorldPosition;
		public float minDistance = 0.1f;
        public PositionChangeRelative worldPositionRelativeOption = PositionChangeRelative.World;
        public Transform worldPositionRelativeTransform;
        public bool checkScreenPosition = true;
		public int minPixelDistance = 10;
		public int maxStepsPerFrame = 12;
		public bool checkTime;
		public float timeInterval = 1f;
		public bool checkCollisions;
		public bool orientToSurface = true;
		public bool ground;
		public float surfaceOffset = 0.05f;
		public LayerMask collisionLayerMask = -1;
		public TrailRenderOrder renderOrder = TrailRenderOrder.DrawBehind;
        [Tooltip("Adds additional render passes to reset stencil after trails have rendered")]
        public bool clearStencil = true;
        [Tooltip("Optional mask texture to be applied to the effect. Uses the red channel as an alpha (transparency) multiplier.")]
		public Texture2D mask;
		public int subMeshMask = -1;
        public UnityEngine.Rendering.CullMode cullMode = UnityEngine.Rendering.CullMode.Back;
        public Gradient colorOverTime;
		public bool colorRamp;
		public Texture2D colorRampTexture;
		public bool fadeOut = true;
		public ColorSequence colorSequence = ColorSequence.Fixed;
        public Color color = Color.white;
		public float colorCycleDuration = 3f;
		public bool colorCycleLoop = true;
		public float pingPongSpeed = 1f;
		public Gradient colorStartPalette;
		public Camera cam;
		public TrailStyle effect = TrailStyle.Color;
		public Material customMaterial;
		public Texture2D texture;
		public Vector3 scale = Vector3.one, scaleStartRandomMin = Vector3.one, scaleStartRandomMax = Vector3.one;
		public bool ignoreTransformScale;
		public AnimationCurve scaleOverTime;
		public bool scaleUniform;
		public Vector3 localPositionRandomMin, localPositionRandomMax;
		public float laserBandWidth = 0.1f, laserIntensity = 20f, laserFlash = 0.2f;
		public Color trailTint = new Color(0f, 0, 0.1f);
        [Tooltip("Fades out effects based on distance to camera")]
        public bool cameraDistanceFade;
        [Tooltip("The closest distance particles can get to the camera before they fade from the camera’s view.")]
        public float cameraDistanceFadeNear;
        [Tooltip("The farthest distance particles can get away from the camera before they fade from the camera’s view.")]
        public float cameraDistanceFadeFar = 1000;
        public Transform lookTarget;
		public bool lookToCamera = true;
		[Range (0, 1)]
		public float textureCutOff = 0.5f;
		[Range (0, 1)]
		public float normalThreshold = 0.3f;
        public OutlineMethod outlineMethod = OutlineMethod.Normals;
        public float rimPower = 2f;
		public bool useLastAnimationState;
		public int maxBatches = 50;
		public int meshPoolSize = 256;
		[Tooltip("Allowed animation states separated by commas")]
		public string animationStates;
		[Tooltip("Interpolate vertices to provide a smoother effect.")]
		public bool interpolate;

        private void OnValidate() {
            rimPower = Mathf.Max(0.1f, rimPower);
        }

        public void Load(TrailEffect effect) {
			effect.active = active;
			effect.ignoreVisibility = ignoreVisibility;
			effect.ignoreFrames = ignoreFrames;
			effect.duration = duration;
			effect.continuous = continuous;
			effect.smooth = smooth;
			effect.checkWorldPosition = checkWorldPosition;
			effect.minDistance = minDistance;
            effect.worldPositionRelativeOption = worldPositionRelativeOption;
            effect.worldPositionRelativeTransform = worldPositionRelativeTransform;
			effect.checkScreenPosition = checkScreenPosition;
			effect.minPixelDistance = minPixelDistance;
			effect.maxStepsPerFrame = maxStepsPerFrame;
			effect.checkTime = checkTime;
			effect.timeInterval = timeInterval;
			effect.checkCollisions = checkCollisions;
			effect.orientToSurface = orientToSurface;
			effect.ground = ground;
			effect.surfaceOffset = surfaceOffset;
			effect.collisionLayerMask = collisionLayerMask;
            effect.cullMode = cullMode;
			effect.subMeshMask = subMeshMask;
			effect.renderOrder = renderOrder;
			effect.clearStencil = clearStencil;
			effect.mask = mask;
			effect.colorOverTime = colorOverTime;
			effect.colorRamp = colorRamp;
			effect.colorRampTexture = colorRampTexture;
			effect.fadeOut = fadeOut;
            effect.color = color;
			effect.colorSequence = colorSequence;
			effect.colorCycleDuration = colorCycleDuration;
			effect.colorStartPalette = colorStartPalette;
			effect.colorCycleLoop = colorCycleLoop;
			effect.trailTint = trailTint;
			effect.cameraDistanceFade = cameraDistanceFade;
			effect.cameraDistanceFadeFar = cameraDistanceFadeFar;
			effect.cameraDistanceFadeNear = cameraDistanceFadeNear;
            effect.pingPongSpeed = pingPongSpeed;
			effect.effect = this.effect;
			effect.customMaterial = customMaterial;
			effect.texture = texture;
			effect.scale = scale;
			effect.scaleStartRandomMin = scaleStartRandomMin;
			effect.scaleStartRandomMax = scaleStartRandomMax;
			effect.scaleOverTime = scaleOverTime;
			effect.scaleUniform = scaleUniform;
			effect.ignoreTransformScale = ignoreTransformScale;
			effect.localPositionRandomMin = localPositionRandomMin;
			effect.localPositionRandomMax = localPositionRandomMax;
			effect.laserBandWidth = laserBandWidth;
			effect.laserIntensity = laserIntensity;
			effect.laserFlash = laserFlash;
			effect.lookTarget = lookTarget;
			effect.lookToCamera = lookToCamera;
			effect.textureCutOff = textureCutOff;
            effect.outlineMethod = outlineMethod;
            effect.rimPower = rimPower;
			effect.normalThreshold = normalThreshold;
			effect.useLastAnimationState = useLastAnimationState;
			effect.maxBatches = maxBatches;
			effect.meshPoolSize = meshPoolSize;
			effect.animationStates = animationStates;
			effect.interpolate = interpolate;
			effect.UpdateMaterialProperties();
		}


		public void Save(TrailEffect effect) {
			active = effect.active;
			ignoreVisibility = effect.ignoreVisibility;
			ignoreFrames = effect.ignoreFrames;
			duration = effect.duration;
			continuous = effect.continuous;
			smooth = effect.smooth;
			checkWorldPosition = effect.checkWorldPosition;
			minDistance = effect.minDistance;
            worldPositionRelativeOption = effect.worldPositionRelativeOption;
            worldPositionRelativeTransform = effect.worldPositionRelativeTransform;
            checkScreenPosition = effect.checkScreenPosition;
			minPixelDistance = effect.minPixelDistance;
			maxStepsPerFrame = effect.maxStepsPerFrame;
			checkTime = effect.checkTime;
			timeInterval = effect.timeInterval;
			checkCollisions = effect.checkCollisions;
			orientToSurface = effect.orientToSurface;
			ground = effect.ground;
			surfaceOffset = effect.surfaceOffset;
			collisionLayerMask = effect.collisionLayerMask;
            renderOrder = effect.renderOrder;
			clearStencil = effect.clearStencil;
			mask = effect.mask;
            cullMode = effect.cullMode;
			subMeshMask = effect.subMeshMask;
			colorOverTime = effect.colorOverTime;
			colorRamp = effect.colorRamp;
			colorRampTexture = effect.colorRampTexture;
			fadeOut = effect.fadeOut;
            color = effect.color;
            colorSequence = effect.colorSequence;
			colorCycleDuration = effect.colorCycleDuration;
			colorCycleLoop = effect.colorCycleLoop;
			colorStartPalette = effect.colorStartPalette;
			trailTint = effect.trailTint;
            cameraDistanceFade = effect.cameraDistanceFade;
            cameraDistanceFadeFar = effect.cameraDistanceFadeFar;
            cameraDistanceFadeNear = effect.cameraDistanceFadeNear;
            pingPongSpeed = effect.pingPongSpeed;
			this.effect = effect.effect;
			customMaterial = effect.customMaterial;
			texture = effect.texture;
			scale = effect.scale;
			scaleStartRandomMin = effect.scaleStartRandomMin;
			scaleStartRandomMax = effect.scaleStartRandomMax;
			scaleOverTime = effect.scaleOverTime;
			scaleUniform = effect.scaleUniform;
			ignoreTransformScale = effect.ignoreTransformScale;
			localPositionRandomMin = effect.localPositionRandomMin;
			localPositionRandomMax = effect.localPositionRandomMax;
			laserBandWidth = effect.laserBandWidth;
			laserIntensity = effect.laserIntensity;
			laserFlash = effect.laserFlash;
			lookTarget = effect.lookTarget;
			lookToCamera = effect.lookToCamera;
			textureCutOff = effect.textureCutOff;
            outlineMethod = effect.outlineMethod;
            rimPower = effect.rimPower;
			normalThreshold = effect.normalThreshold;
			useLastAnimationState = effect.useLastAnimationState;
			maxBatches = effect.maxBatches;
			meshPoolSize = effect.meshPoolSize;
			animationStates = effect.animationStates;
			interpolate = effect.interpolate;
		}
	}



}

