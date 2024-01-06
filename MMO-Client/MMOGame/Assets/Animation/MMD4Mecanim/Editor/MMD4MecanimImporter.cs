using UnityEditor;

[InitializeOnLoad]
public class MMD4MecanimVersionChecker
{
	static MMD4MecanimVersionChecker()
	{
		MMD4MecanimEditorCommon.RestoreLibraries();
	}
}

public class MMD4MecanimImporter : MMD4MecanimImporterImpl
{
}
