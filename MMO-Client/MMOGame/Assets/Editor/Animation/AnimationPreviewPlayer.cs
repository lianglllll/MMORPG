using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorScripts
{

    [InitializeOnLoad]
    public static class AnimationPreviewPlayer
    {

        private const BindingFlags PRIVATE_FIELD_BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
        private const BindingFlags PUBLIC_FIELD_BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField;
        private const BindingFlags PUBLIC_PROPERTY_BINDING_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        private static Type animationClipEditorType;
        private static Type avatarPreviewType;
        private static Type timeControlType;

        private static Object selectedObject;
        private static bool shouldFindTimeControl;

        static AnimationPreviewPlayer()
        {
            animationClipEditorType = Type.GetType("UnityEditor.AnimationClipEditor,UnityEditor");
            avatarPreviewType = Type.GetType("UnityEditor.AvatarPreview,UnityEditor");
            timeControlType = Type.GetType("UnityEditor.TimeControl,UnityEditor");
        }

        private static void Update()
        {
            if (Selection.activeObject != selectedObject)
            {
                selectedObject = Selection.activeObject;

                if (selectedObject is AnimationClip)
                {
                    shouldFindTimeControl = true;
                }
                else if (selectedObject is GameObject)
                {
                    var assetPath = AssetDatabase.GetAssetPath(selectedObject);

                    if (!string.IsNullOrWhiteSpace(assetPath))
                    {
                        foreach (var child in AssetDatabase.LoadAllAssetsAtPath(assetPath))
                        {
                            if (child is AnimationClip)
                            {
                                shouldFindTimeControl = true;
                                break;
                            }
                        }
                    }
                }

                return;
            }

            if (shouldFindTimeControl)
            {
                var animationClipEditor = Resources.FindObjectsOfTypeAll(animationClipEditorType).FirstOrDefault();
                if (animationClipEditor == null) return;

                var avatarPreview = animationClipEditorType.GetField("m_AvatarPreview", PRIVATE_FIELD_BINDING_FLAGS)?.GetValue(animationClipEditor);
                if (avatarPreview == null) return;

                var timeControl = avatarPreviewType.GetField("timeControl", PUBLIC_FIELD_BINDING_FLAGS)?.GetValue(avatarPreview);
                if (timeControl == null) return;

                shouldFindTimeControl = false;

                var playingProperty = timeControlType.GetProperty("playing", PUBLIC_PROPERTY_BINDING_FLAGS);
                if (playingProperty == null) return;

                playingProperty.SetValue(timeControl, true);
            }
        }

        [InitializeOnLoadMethod, UsedImplicitly]
        private static void SubscribeToUpdate()
        {
            EditorApplication.update += Update;
        }

    }

}
