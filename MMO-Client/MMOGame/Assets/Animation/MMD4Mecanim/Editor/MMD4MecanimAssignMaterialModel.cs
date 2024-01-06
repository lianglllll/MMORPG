using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

class MMD4MecanimAssignMaterialModel : AssetPostprocessor
{
    #if UNITY_2017_2_OR_NEWER
    Material OnAssignMaterialModel( Material material, Renderer renderer )
    {
        int len = assetPath.Length;
        if( len >= 4 ) {
            // Note: Faster than method.
            char c0 = assetPath[len - 4], c1 = assetPath[len - 3], c2 = assetPath[len - 2], c3 = assetPath[len - 1];
            if( (c0 == '.') &&
                (c1 == 'f' || c1 == 'F') ||
                (c2 == 'b' || c2 == 'B') ||
                (c3 == 'x' || c3 == 'X') ) {
                string basePath = assetPath.Substring(0, len - 3);
                if( File.Exists( basePath + "pmd" ) || File.Exists( basePath + "pmx" ) ) {
                    Material newMaterial = ExtractMaterials( assetPath, material );
                    if( newMaterial != null ) {
                        AssetDatabase.WriteImportSettingsIfDirty( assetPath );
                    }

                    return newMaterial;
                }
            }
        }

        return null;
    }
    #endif // UNITY_2017_2_OR_NEWER

    public static Material ExtractMaterials( string assetPath, Material material )
    {
        #if UNITY_2017_2_OR_NEWER
        if( assetPath == null || material == null ) {
            return null;
        }

        string materialName = material.name;
        if( string.IsNullOrEmpty( materialName ) ) {
            return null;
        }

		var assetImporter = AssetImporter.GetAtPath( assetPath );
        if( assetImporter == null ) {
            return null;
        }

        var externalObjectMap = assetImporter.GetExternalObjectMap();
        var sourceAssetIdentifer = new AssetImporter.SourceAssetIdentifier( typeof(Material), materialName );
        if( externalObjectMap != null && externalObjectMap.ContainsKey( sourceAssetIdentifer ) ) {
            return null;
        }

        var materialsPath = PathCombine( Path.GetDirectoryName( assetPath ), "Materials" );
        if( !Directory.Exists( materialsPath ) ) {
            try {
                Directory.CreateDirectory( materialsPath );
            } catch( System.Exception e ) {
                Debug.LogError( e.ToString() );
                return null;
            }
        }

        string materialPath = PathCombine( materialsPath, EscapeFileName( materialName ) + ".mat" );

        try {
            Material newMaterial = null;
            if( !File.Exists( materialPath ) ) {
                newMaterial = new Material( material );
                AssetDatabase.CreateAsset( newMaterial, materialPath );
            } else {
                newMaterial = AssetDatabase.LoadAssetAtPath( materialPath, typeof(Material) ) as Material;
            }

            assetImporter.AddRemap( sourceAssetIdentifer, newMaterial );
            return newMaterial;
        } catch( System.Exception e ) {
            Debug.LogError( e.ToString() );
            return null;
        }
        #else // UNITY_2017_2_OR_NEWER
        return null;
        #endif// UNITY_2017_2_OR_NEWER
    }
    
    public static string PathCombine( string pathA, string pathB )
    {
        if( pathA == null ) {
            return pathB;
        } else if( pathB == null ) {
            return pathA;
        }

        int lenA = pathA.Length;
        if( lenA == 0 ) {
            return pathB;
        }
        int lenB = pathB.Length;
        if( lenB == 0 ) {
            return pathA;
        }

        if( pathA[lenA - 1] == '\\' || pathA[lenA - 1] == '/' ) {
            return pathA + pathB;
        }
        if( pathB[0] == '\\' || pathB[0] == '/' ) {
            return pathA + pathB;
        }

        for( int i = 0; i < lenA; ++i ) {
            char c = pathA[i];
            if( c == '\\' ) {
                return pathA + '\\' + pathB;
            }
            if( c == '/' ) {
                return pathA + '/' + pathB;
            }
        }

        return pathA + System.IO.Path.DirectorySeparatorChar + pathB;
    }

    public static string EscapeFileName( string fileName )
    {
        System.Text.StringBuilder r = new System.Text.StringBuilder();
        for( int i = 0, len = fileName.Length; i < len; ++i ) {
            char c = fileName[i];
		    if( c == '\t' ||
			    c == '\\' ||
			    c == '/'  ||
			    c == ':'  ||
			    c == '*'  ||
			    c == '?'  ||
			    c == '\"' ||
			    c == '<'  ||
			    c == '>'  ||
			    c == '|' ) {
			    r.Append( '_' );
		    } else {
			    if( c != '.' || i + 1 < len ) {
    			    r.Append( c );
			    } else {
    			    r.Append( '_' );
			    }
		    }
        }

        return r.ToString();
    }
}
