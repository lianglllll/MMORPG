using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_5_6_OR_NEWER
public class MMD4MecanimPreProcessBuild
#if UNITY_2018_1_OR_NEWER
	: UnityEditor.Build.IPreprocessBuildWithReport
#else
	: UnityEditor.Build.IPreprocessBuild
#endif
{
    public int callbackOrder { get { return 0; } }
	#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        if( report.summary.platform == BuildTarget.Android ) {
            MMD4MecanimEditorCommon.CheckAndroidDevices( true );
        }
    }
	#else
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if( target == BuildTarget.Android ) {
            MMD4MecanimEditorCommon.CheckAndroidDevices( true );
        }
    }
	#endif
}
#endif
