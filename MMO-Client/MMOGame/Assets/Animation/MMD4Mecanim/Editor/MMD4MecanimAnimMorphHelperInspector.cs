using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[CustomEditor(typeof(MMD4MecanimAnimMorphHelper))]
public class MMD4MecanimAnimMorphHelperInspector : Editor
{
	public override void OnInspectorGUI()
	{
		MMD4MecanimAnimMorphHelper animMorphHelper = this.target as MMD4MecanimAnimMorphHelper;

		EditorGUILayout.Separator();

		animMorphHelper.animName = EditorGUILayout.TextField( "Anim Name", animMorphHelper.animName );
		animMorphHelper.playingAnimName = EditorGUILayout.TextField( "Playing Anim Name", animMorphHelper.playingAnimName );

		EditorGUILayout.Separator();

		animMorphHelper.animEnabled = EditorGUILayout.Toggle( "Enabled", animMorphHelper.animEnabled );
		animMorphHelper.animPauseOnEnd = EditorGUILayout.Toggle( "Pause On End", animMorphHelper.animPauseOnEnd );
		animMorphHelper.animSyncToAudio = EditorGUILayout.Toggle( "Sync To Audio", animMorphHelper.animSyncToAudio );

		EditorGUILayout.Separator();

		animMorphHelper.initializeOnAwake = EditorGUILayout.Toggle( "Initialize On Awake", animMorphHelper.initializeOnAwake );
		animMorphHelper.morphSpeed = EditorGUILayout.FloatField( "Morph Speed (Start/End)", animMorphHelper.morphSpeed );
		animMorphHelper.overrideWeight = EditorGUILayout.Toggle( "Override Weight", animMorphHelper.overrideWeight );

		EditorGUILayout.Separator();

		if( animMorphHelper.animList != null ) {
			GUILayout.Label( "Animations", EditorStyles.boldLabel );

			for( int animIndex = 0; animIndex < animMorphHelper.animList.Length; ) {
				MMD4MecanimAnimMorphHelper.Anim anim = animMorphHelper.animList[animIndex];

				EditorGUILayout.BeginHorizontal();
				bool isRemove = GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false) );
				anim.animName = EditorGUILayout.TextField( "Anim Name", anim.animName );
				EditorGUILayout.EndHorizontal();

				TextAsset animFile = anim.animFile;
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(26.0f);
				animFile = (TextAsset)EditorGUILayout.ObjectField( "Anim File", (Object)animFile, typeof(TextAsset), false );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(26.0f);
				anim.audioClip = (AudioClip)EditorGUILayout.ObjectField( "Audio Clip", (AudioClip)anim.audioClip, typeof(AudioClip), false );
				EditorGUILayout.EndHorizontal();

				if( animFile != null ) {
					if( !AssetDatabase.GetAssetPath( animFile ).ToLower().EndsWith( ".anim.bytes" ) ) {
						animFile = null;
					} else {
						if( anim.animFile != animFile ) {
							anim.animFile = animFile;
							anim.animName = System.IO.Path.GetFileNameWithoutExtension( animFile.name );
						}
					}
				} else {
					isRemove = true;
					anim.animFile = null;
				}
				if( isRemove ) {
					for( int i = animIndex; i + 1 < animMorphHelper.animList.Length; ++i ) {
						animMorphHelper.animList[i] = animMorphHelper.animList[i + 1];
					}
					System.Array.Resize( ref animMorphHelper.animList, animMorphHelper.animList.Length - 1 );
				} else {
					++animIndex;
				}
			}
		}
			
		EditorGUILayout.Separator();
			
		{
			GUILayout.Label( "Add Animation", EditorStyles.boldLabel );
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(26.0f);
			TextAsset animFile = (TextAsset)EditorGUILayout.ObjectField( "Anim File", (Object)null, typeof(TextAsset), false );
			EditorGUILayout.EndHorizontal();
			if( animFile != null ) {
				if( !AssetDatabase.GetAssetPath( animFile ).ToLower().EndsWith( ".anim.bytes" ) ) {
					Debug.LogWarning( System.IO.Path.GetExtension( AssetDatabase.GetAssetPath( animFile ) ).ToLower() );
					animFile = null;
				} else {
					MMD4MecanimAnimMorphHelper.Anim anim = new MMD4MecanimAnimMorphHelper.Anim();
					anim.animFile = animFile;
					anim.animName = System.IO.Path.GetFileNameWithoutExtension( animFile.name );
					if( animMorphHelper.animList == null ) {
						animMorphHelper.animList = new MMD4MecanimAnimMorphHelper.Anim[1];
						animMorphHelper.animList[0] = anim;
					} else {
						int animIndex = animMorphHelper.animList.Length;
						System.Array.Resize( ref animMorphHelper.animList, animIndex + 1 );
						animMorphHelper.animList[animIndex] = anim;
					}
				}
			}
		}
	}
}
