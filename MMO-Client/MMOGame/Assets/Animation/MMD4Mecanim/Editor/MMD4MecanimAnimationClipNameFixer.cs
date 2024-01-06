using UnityEngine;
using UnityEditor;

#if UNITY_2018_1_OR_NEWER || UNITY_2019_1_OR_NEWER || UNITY_2020_1_OR_NEWER || UNITY_2021_1_OR_NEWER || UNITY_2022_1_OR_NEWER || UNITY_2023_1_OR_NEWER || UNITY_2024_1_OR_NEWER
public class MMD4MecanimAnimationClipNameFixer : AssetPostprocessor
{
	[InitializeOnLoadMethod]
	static void OnInitialize()
	{
		if( CheckOnBootEditor() ) {
			FixAnimationClipName( AssetDatabase.GetAllAssetPaths() );
		}
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		FixAnimationClipName( importedAssets );
	}

	static void FixAnimationClipName( string[] assetPaths )
	{
		if( assetPaths != null ) {
			foreach( var assetPath in assetPaths ) {
				if( IsExistsMMD4MecanimAsset( assetPath ) ) {
					FixAnimationClipName( assetPath );
				}
			}
		}
	}

	const string LockTempFileName = "Temp/_m4macnf.tmp";

	static bool CheckOnBootEditor()
	{
		if( System.IO.File.Exists( LockTempFileName ) ) {
			return false;
		}

		System.IO.File.WriteAllText( LockTempFileName, "" );
		return true;
	}

	static bool IsFBX( string assetPath )
	{
		if( assetPath == null ) {
			return false;
		}

		var pathLength = assetPath.Length;
		if( pathLength <= 4 ) {
			return false; // Failsafe.
		}

		if( assetPath[pathLength - 4] == '.' &&
			(assetPath[pathLength - 3] == 'F' || assetPath[pathLength - 3] == 'f') &&
			(assetPath[pathLength - 2] == 'B' || assetPath[pathLength - 2] == 'b') &&
			(assetPath[pathLength - 1] == 'X' || assetPath[pathLength - 1] == 'x') ) {
			return true;
		}

		return false;
	}

	static bool IsExistsMMD4MecanimAsset( string assetPath )
	{
		if( !IsFBX( assetPath ) ) {
			return false;
		}

		var baseName = assetPath.Substring( 0, assetPath.Length - 4 );

		return System.IO.File.Exists( baseName + ".MMD4Mecanim.asset" );
	}

	static void FixAnimationClipName( string assetPath )
	{ 
		if( string.IsNullOrEmpty( assetPath ) ) {
			return;
		}

		ModelImporter modelImporter = (ModelImporter)ModelImporter.GetAtPath( assetPath );
		if( modelImporter == null ) {
			return;
		}
		
		var clipAnimations = modelImporter.clipAnimations;
		var defaultClipAnimations = modelImporter.defaultClipAnimations;
		if( UpdateModelImporterClipAnimations( ref clipAnimations, defaultClipAnimations ) ) {
			modelImporter.clipAnimations = clipAnimations;
			modelImporter.SaveAndReimport();
		}
	}

	static bool ConvertToLegacyAnimationClipName( ref string name )
	{
		if( string.IsNullOrEmpty( name ) ) {
			return false;
		}

		int idx0 = name.LastIndexOf("/");
		int idx1 = name.LastIndexOf("\\");

		int idx;
		if(idx0 >= 0 && idx1 >= 0) {
			idx = Mathf.Max(idx0, idx1);
		} else if(idx0 >= 0) {
			idx = idx0;
		} else if(idx1 >= 0) {
			idx = idx1;
		} else {
			return false;
		}

		if(idx + 1 >= name.Length) {
			return false;
		}

		name = name.Substring(idx + 1);
		return true;
	}

	static bool UpdateModelImporterClipAnimations( ref ModelImporterClipAnimation[] clipAnimations, ModelImporterClipAnimation[] defaultClipAnimations )
	{
		if( defaultClipAnimations == null || defaultClipAnimations.Length == 0 ) {
			return false;
		}

		bool updatedAnything = false;

		if( clipAnimations != null ) {
			foreach( var clipAnimation in clipAnimations ) {
				if( clipAnimation != null ) {
					string name = clipAnimation.name;
					if( ConvertToLegacyAnimationClipName( ref name ) ) {
						clipAnimation.name = name;
						updatedAnything = true;
					}
				}
			}
		}

		if( defaultClipAnimations != null ) {
			foreach( var defaultClipAnimation in defaultClipAnimations ) {
				if( defaultClipAnimation != null ) {
					string name = defaultClipAnimation.name;
					if( ConvertToLegacyAnimationClipName( ref name ) ) {
						defaultClipAnimation.name = name;
						updatedAnything |= WeakAddClipAnimation( ref clipAnimations, defaultClipAnimation );
					}
				}
			}
		}

		return updatedAnything;
	}

	static bool WeakAddClipAnimation( ref ModelImporterClipAnimation[] clipAnimations, ModelImporterClipAnimation clipAnimation )
	{ 
		if( clipAnimation == null ) {
			return false;
		}

		if( clipAnimations == null || clipAnimations.Length == 0 ) {
			clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
			return true;
		}

		foreach( var t in clipAnimations ) {
			if( t != null ) {
				if( t.name == clipAnimation.name ) {
					return false;
				}
			}
		}

		int index = clipAnimations.Length;
		System.Array.Resize( ref clipAnimations, index + 1 );
		clipAnimations[index] = clipAnimation;
		return true;
	}
}
#endif
