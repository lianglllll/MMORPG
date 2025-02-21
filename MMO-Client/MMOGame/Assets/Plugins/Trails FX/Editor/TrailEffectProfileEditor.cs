﻿using UnityEditor;
using UnityEngine;

namespace TrailsFX {

    [CustomEditor(typeof(TrailEffectProfile))]
    [CanEditMultipleObjects]
    public class TrailEffectProfileEditor : Editor {
        SerializedProperty active, ignoreVisibility, ignoreFrames;
        SerializedProperty continuous, smooth;
        SerializedProperty checkWorldPosition, minDistance, worldPositionRelativeOption, worldPositionRelativeTransform;
        SerializedProperty checkScreenPosition, minPixelDistance;
        SerializedProperty checkTime, timeInterval;
        SerializedProperty checkCollisions, collisionLayerMask, orientToSurface, surfaceOffset;
        SerializedProperty duration, maxStepsPerFrame;
        SerializedProperty mask;
        SerializedProperty renderOrder, clearStencil, cullMode, subMeshMask, lookTarget, lookToCamera, ground;
        SerializedProperty colorSequence, color, colorCycleDuration, colorCycleLoop, pingPongSpeed, colorStartPalette, colorOverTime, colorRamp, colorRampTexture, fadeOut;
        SerializedProperty laserBandWidth, laserIntensity, laserFlash;
        SerializedProperty trailTint;
        SerializedProperty effect, customMaterial, texture, textureCutOff;
        SerializedProperty localPositionRandomMin, localPositionRandomMax;
        SerializedProperty scale, scaleStartRandomMin, scaleStartRandomMax, scaleOverTime, scaleUniform, ignoreTransformScale;
        SerializedProperty outlineMethod, normalThreshold, rimPower;
        SerializedProperty useLastAnimationState, maxBatches, meshPoolSize, interpolate, animationStates;
        SerializedProperty cameraDistanceFade, cameraDistanceFadeNear, cameraDistanceFadeFar;

        void OnEnable() {
            active = serializedObject.FindProperty("active");
            ignoreVisibility = serializedObject.FindProperty("ignoreVisibility");
            continuous = serializedObject.FindProperty("continuous");
            smooth = serializedObject.FindProperty("smooth");
            ignoreFrames = serializedObject.FindProperty("ignoreFrames");
            checkWorldPosition = serializedObject.FindProperty("checkWorldPosition");
            minDistance = serializedObject.FindProperty("minDistance");
            worldPositionRelativeOption = serializedObject.FindProperty("worldPositionRelativeOption");
            worldPositionRelativeTransform = serializedObject.FindProperty("worldPositionRelativeTransform");
            checkScreenPosition = serializedObject.FindProperty("checkScreenPosition");
            minPixelDistance = serializedObject.FindProperty("minPixelDistance");
            checkTime = serializedObject.FindProperty("checkTime");
            checkCollisions = serializedObject.FindProperty("checkCollisions");
            timeInterval = serializedObject.FindProperty("timeInterval");
            collisionLayerMask = serializedObject.FindProperty("collisionLayerMask");
            orientToSurface = serializedObject.FindProperty("orientToSurface");
            surfaceOffset = serializedObject.FindProperty("surfaceOffset");
            duration = serializedObject.FindProperty("duration");
            maxStepsPerFrame = serializedObject.FindProperty("maxStepsPerFrame");
            mask = serializedObject.FindProperty("mask");
            renderOrder = serializedObject.FindProperty("renderOrder");
            clearStencil = serializedObject.FindProperty("clearStencil");
            cameraDistanceFade = serializedObject.FindProperty("cameraDistanceFade");
            cameraDistanceFadeNear = serializedObject.FindProperty("cameraDistanceFadeNear");
            cameraDistanceFadeFar = serializedObject.FindProperty("cameraDistanceFadeFar"); cullMode = serializedObject.FindProperty("cullMode");
            subMeshMask = serializedObject.FindProperty("subMeshMask");
            colorSequence = serializedObject.FindProperty("colorSequence");
            colorCycleDuration = serializedObject.FindProperty("colorCycleDuration");
            colorCycleLoop = serializedObject.FindProperty("colorCycleLoop");
            pingPongSpeed = serializedObject.FindProperty("pingPongSpeed");
            color = serializedObject.FindProperty("color");
            colorStartPalette = serializedObject.FindProperty("colorStartPalette");
            colorOverTime = serializedObject.FindProperty("colorOverTime");
            colorRamp = serializedObject.FindProperty("colorRamp");
            colorRampTexture = serializedObject.FindProperty("colorRampTexture");
            fadeOut = serializedObject.FindProperty("fadeOut");
            scaleOverTime = serializedObject.FindProperty("scaleOverTime");
            effect = serializedObject.FindProperty("effect");
            customMaterial = serializedObject.FindProperty("customMaterial");
            texture = serializedObject.FindProperty("texture");
            textureCutOff = serializedObject.FindProperty("textureCutOff");
            localPositionRandomMin = serializedObject.FindProperty("localPositionRandomMin");
            localPositionRandomMax = serializedObject.FindProperty("localPositionRandomMax");
            scale = serializedObject.FindProperty("scale");
            scaleStartRandomMin = serializedObject.FindProperty("scaleStartRandomMin");
            scaleStartRandomMax = serializedObject.FindProperty("scaleStartRandomMax");
            scaleUniform = serializedObject.FindProperty("scaleUniform");
            ignoreTransformScale = serializedObject.FindProperty("ignoreTransformScale");
            lookTarget = serializedObject.FindProperty("lookTarget");
            lookToCamera = serializedObject.FindProperty("lookToCamera");
            ground = serializedObject.FindProperty("ground");
            useLastAnimationState = serializedObject.FindProperty("useLastAnimationState");
            maxBatches = serializedObject.FindProperty("maxBatches");
            meshPoolSize = serializedObject.FindProperty("meshPoolSize");
            interpolate = serializedObject.FindProperty("interpolate");
            animationStates = serializedObject.FindProperty("animationStates");
            outlineMethod = serializedObject.FindProperty("outlineMethod");
            rimPower = serializedObject.FindProperty("rimPower");
            normalThreshold = serializedObject.FindProperty("normalThreshold");
            laserBandWidth = serializedObject.FindProperty("laserBandWidth");
            laserIntensity = serializedObject.FindProperty("laserIntensity");
            laserFlash = serializedObject.FindProperty("laserFlash");
            trailTint = serializedObject.FindProperty("trailTint");
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.Separator();
            serializedObject.Update();

            EditorGUILayout.LabelField("Triggers", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(active, new GUIContent("Active", "Enable new trails."));
            EditorGUILayout.PropertyField(ignoreVisibility);
            EditorGUILayout.PropertyField(ignoreFrames, new GUIContent("Ignore First Frames", "Number of initial frames to ignore."));
            EditorGUILayout.PropertyField(continuous, new GUIContent("Continuous", "Continuous trail."));
            if (!continuous.boolValue) {
                EditorGUILayout.PropertyField(checkWorldPosition, new GUIContent("World Position Change", "Adds a trail when the position of the object in world space changes."));
                if (checkWorldPosition.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(minDistance);
                    EditorGUILayout.PropertyField(smooth);
                    EditorGUILayout.PropertyField(worldPositionRelativeOption, new GUIContent("Relative To", "You can optionally specify a reference position or pivot determined by another gameobject. This can be useful if you want to track position changes with respect to a parent object."));
                    if (worldPositionRelativeOption.intValue == (int)PositionChangeRelative.OtherGameObject) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(worldPositionRelativeTransform, new GUIContent("GameObject"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(checkScreenPosition, new GUIContent("Screen Position Change", "Adds a trail when the screen position of the object changes."));
                if (checkScreenPosition.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(minPixelDistance);
                    EditorGUILayout.PropertyField(smooth);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(checkTime, new GUIContent("Time Interval"));
                if (checkTime.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(timeInterval, new GUIContent("Interval", "Interval in seconds."));
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.PropertyField(checkCollisions, new GUIContent("Collisions"));
            if (checkCollisions.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(collisionLayerMask, new GUIContent("Layer Mask"));
                EditorGUILayout.PropertyField(orientToSurface, new GUIContent("Orient To Surface"));
                EditorGUILayout.PropertyField(surfaceOffset, new GUIContent("Surface Offset"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(mask, new GUIContent("Mask (R)"));
            EditorGUILayout.PropertyField(renderOrder);
            EditorGUILayout.PropertyField(clearStencil);
            EditorGUILayout.PropertyField(cameraDistanceFade);
            if (cameraDistanceFade.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(cameraDistanceFadeNear, new GUIContent("Near Distance"));
                EditorGUILayout.PropertyField(cameraDistanceFadeFar, new GUIContent("Far Distance"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(cullMode, new GUIContent("Cull Mode", "Culling mode for trail rendering."));
            EditorGUILayout.PropertyField(subMeshMask, new GUIContent("SubMesh Mask", "Specify which submeshes are included in the trail. This is a bitmask: -1 = all (default value), 1 = first submesh only, 2 = second submesh only, 3 means first and second submesh only, 4 = third submesh only, and so on."));

            EditorGUILayout.PropertyField(maxStepsPerFrame, new GUIContent("Max Steps Per Frame", "If the object moves fast, the object is drawn several times along the trajectory depending on the trigger options Min Distance and Min Pixel Distance. This property controls the maximum number of repetitions per frame."));
            EditorGUILayout.PropertyField(effect, new GUIContent("Trail Effect"));
            EditorGUI.indentLevel++;
            switch (effect.intValue) {
                case (int)TrailStyle.Color:
                    EditorGUILayout.PropertyField(colorRamp, new GUIContent("Color Ramp"));
                    if (colorRamp.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(colorRampTexture, new GUIContent("Color Ramp Texture"));
                        EditorGUI.indentLevel--;
                    }
                    break;
                case (int)TrailStyle.TextureStamp:
                    EditorGUILayout.PropertyField(texture);
                    EditorGUILayout.PropertyField(textureCutOff, new GUIContent("Cut Off"));
                    EditorGUILayout.PropertyField(lookTarget, new GUIContent("Look Target"));
                    if (lookTarget.objectReferenceValue == null) {
                        EditorGUILayout.PropertyField(lookToCamera, new GUIContent("Look To Camera"));
                    }
                    break;
                case (int)TrailStyle.Clone:
                    EditorGUILayout.PropertyField(textureCutOff, new GUIContent("Luminance Cut Off"));
                    break;
                case (int)TrailStyle.Outline:
                    EditorGUILayout.PropertyField(outlineMethod, new GUIContent("Method"));
                    if (outlineMethod.intValue == (int)OutlineMethod.Normals) {
                        EditorGUILayout.PropertyField(normalThreshold, new GUIContent("Normal Threshold"));
                    } else {
                        EditorGUILayout.PropertyField(rimPower, new GUIContent("Rim Power"));
                    }
                    break;
                case (int)TrailStyle.Dash:
                    EditorGUILayout.PropertyField(laserBandWidth, new GUIContent("Separation"));
                    EditorGUILayout.PropertyField(laserIntensity, new GUIContent("Intensity"));
                    EditorGUILayout.PropertyField(laserFlash, new GUIContent("Flash"));
                    break;
                case (int)TrailStyle.SpaceDistortion:
                    EditorGUILayout.PropertyField(trailTint, new GUIContent("Additive Tint"));
                    break;
                case (int)TrailStyle.Custom:
                    EditorGUILayout.PropertyField(customMaterial);
                    break;
            }

            if (((TrailStyle)effect.intValue).supportsColor()) {
                EditorGUILayout.PropertyField(colorSequence, new GUIContent("Color"));
                EditorGUI.indentLevel++;
                switch (colorSequence.intValue) {
                    case (int)ColorSequence.Cycle:
                        EditorGUILayout.PropertyField(colorStartPalette, new GUIContent("Palette"));
                        EditorGUILayout.PropertyField(colorCycleDuration, new GUIContent("Cycle Duration"));
                        EditorGUILayout.PropertyField(colorCycleLoop, new GUIContent("Loop"));
                        break;
                    case (int)ColorSequence.PingPong:
                        EditorGUILayout.PropertyField(colorStartPalette, new GUIContent("Palette"));
                        EditorGUILayout.PropertyField(pingPongSpeed, new GUIContent("Speed"));
                        break;
                    case (int)ColorSequence.Random:
                    case (int)ColorSequence.FixedRandom:
                        EditorGUILayout.PropertyField(colorStartPalette, new GUIContent("Palette"));
                        break;
                    case (int)ColorSequence.Fixed:
                        EditorGUILayout.PropertyField(color);
                        break;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(colorOverTime, new GUIContent("Color Over Time", "Tint color and alpha to be multiplied to the start color."));
                EditorGUILayout.PropertyField(colorRamp, new GUIContent("Stylized Color Ramp"));
            }
            EditorGUILayout.PropertyField(fadeOut, new GUIContent("Fade Out", "Applies fade out to the trail color over time."));

            EditorGUILayout.LabelField(new GUIContent("Position"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(localPositionRandomMin, new GUIContent("Random Min"));
            EditorGUILayout.PropertyField(localPositionRandomMax, new GUIContent("Random Max"));
            EditorGUILayout.PropertyField(ground, new GUIContent("Ground", "Position the texture at the ground beneath the object"));
            if (ground.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(orientToSurface, new GUIContent("Orient To Surface"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(new GUIContent("Scale"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(ignoreTransformScale);
            EditorGUILayout.PropertyField(scaleUniform, new GUIContent("Uniform Scale"));
            if (scaleUniform.boolValue) {
				Vector3 baseScale = scale.vector3Value;
				baseScale.x = EditorGUILayout.FloatField (new GUIContent ("Base Scale"), baseScale.x);
				baseScale.y = baseScale.z = baseScale.x;
				scale.vector3Value = baseScale;
                Vector3 min = scaleStartRandomMin.vector3Value;
                min.x = EditorGUILayout.FloatField(new GUIContent("Random Min"), min.x);
                scaleStartRandomMin.vector3Value = min;
                Vector3 max = scaleStartRandomMax.vector3Value;
                max.x = EditorGUILayout.FloatField(new GUIContent("Random Max"), max.x);
                scaleStartRandomMax.vector3Value = max;
            } else {
				EditorGUILayout.PropertyField(scale, new GUIContent("Base Scale"));
                EditorGUILayout.PropertyField(scaleStartRandomMin, new GUIContent("Random Min"));
                EditorGUILayout.PropertyField(scaleStartRandomMax, new GUIContent("Random Max"));
            }
            EditorGUILayout.PropertyField(scaleOverTime, new GUIContent("Scale Over Time"));
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Skinned Mesh / Particle Systems Only");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(useLastAnimationState, new GUIContent("Use Last Animation", "Repeats the last animation for the entire trail. This option optimizes the number of batches."));
            if (!useLastAnimationState.boolValue) {
                EditorGUILayout.PropertyField(maxBatches, new GUIContent("Max Batches", "Can specify the maximum number of batches allowed when rendering animated skinned mesh trails."));
                EditorGUILayout.PropertyField(meshPoolSize, new GUIContent("Mesh Pool Size", "Maximum number of baked skinned meshes. Reducing this value will save memory but very long trails may reuse animation steps."));
                EditorGUILayout.PropertyField(interpolate, new GUIContent("Interpolate", "Interpolate vertices to provide a smoother effect."));
            }
            EditorGUILayout.PropertyField(animationStates, new GUIContent("Animation States", "Add trails only during these animation states."));
            EditorGUI.indentLevel--;

            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ExecuteCommand &&
                Event.current.commandName == "UndoRedoPerformed")) {

                // Triggers profile reload on all Trail Effect scripts
                TrailEffect[] effects = FindObjectsOfType<TrailEffect>();
                for (int t = 0; t < targets.Length; t++) {
                    TrailEffectProfile profile = (TrailEffectProfile)targets[t];
                    for (int k = 0; k < effects.Length; k++) {
                        if (effects[k] != null && effects[k].profile == profile && effects[k].profileSync) {
                            profile.Load(effects[k]);
                            effects[k].UpdateMaterialProperties();
                        }
                    }
                }
                EditorUtility.SetDirty(target);
            }
        }



    }

}