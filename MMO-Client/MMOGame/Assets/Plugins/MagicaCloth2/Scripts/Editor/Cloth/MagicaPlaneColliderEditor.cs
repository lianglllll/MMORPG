﻿// Magica Cloth 2.
// Copyright (c) 2023 MagicaSoft.
// https://magicasoft.jp
using UnityEditor;

namespace MagicaCloth2
{
    /// <summary>
    /// PlaneColliderのインスペクター拡張
    /// </summary>
    [CustomEditor(typeof(MagicaPlaneCollider))]
    [CanEditMultipleObjects]
    public class MagicaPlaneColliderEditor : MagicaEditorBase
    {
        public override void OnInspectorGUI()
        {
            var scr = target as MagicaPlaneCollider;

            serializedObject.Update();
            Undo.RecordObject(scr, "PlaneCollider");

            // center
            EditorGUILayout.PropertyField(serializedObject.FindProperty("center"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
