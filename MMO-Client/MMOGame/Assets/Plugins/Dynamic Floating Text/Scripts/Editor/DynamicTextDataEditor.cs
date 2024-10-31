using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicTextData))]
public class DynamicTextDataEditor : Editor
{

    SerializedProperty colours;
    SerializedProperty sizes;
    SerializedProperty font;
    SerializedProperty colourAlternationMode;
    SerializedProperty sizeAlternationMode;
    SerializedProperty enterType;
    SerializedProperty exitType;

    private bool shiftOrBounceFound;
    private bool popFound;

    private void OnEnable()
    {

        colours = serializedObject.FindProperty("colours");
        sizes = serializedObject.FindProperty("sizes");
        font = serializedObject.FindProperty("font");
        colourAlternationMode = serializedObject.FindProperty("colourAlternationMode");
        sizeAlternationMode = serializedObject.FindProperty("sizeAlternationMode");
        enterType = serializedObject.FindProperty("enterType");
        exitType = serializedObject.FindProperty("exitType");

    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        DynamicTextData dynamicTextData = (DynamicTextData)target;

        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Dynamic Text Options:");
        EditorGUILayout.LabelField(" ");

        dynamicTextData.id = EditorGUILayout.TextField(new GUIContent("ID:", "A string to be used as a " +
            " unique identifier. This isn't necessary for functionality, but may be useful if you" +
            " want to store different text data in a database, for example."), dynamicTextData.id);

        EditorGUILayout.LabelField(" ");

        dynamicTextData.lifetime = EditorGUILayout.FloatField(new GUIContent("Lifetime: ", "The time in seconds for which this text is active."), dynamicTextData.lifetime);

        EditorGUILayout.PropertyField(colours, new GUIContent("Colours: ", "An array containing every colour you want your text to cycle through."));

        EditorGUILayout.PropertyField(sizes, new GUIContent("Sizes: ", "An array containing every size you want your text to cycle through."));

        EditorGUILayout.PropertyField(font, new GUIContent("Font: ", "A TMP font asset. Falls back to default font if none is selected here."));

        dynamicTextData.bold = EditorGUILayout.Toggle(new GUIContent("Bold: ", "Choose whether to make this text bold."), dynamicTextData.bold);
        dynamicTextData.italic = EditorGUILayout.Toggle(new GUIContent("Italic: ", "Choose whether to make this text italic."), dynamicTextData.italic);
        dynamicTextData.underline = EditorGUILayout.Toggle(new GUIContent("Underline: ", "Choose whether to make this text underlined."), dynamicTextData.underline);
        dynamicTextData.strikethrough = EditorGUILayout.Toggle(new GUIContent("Strikethrough: ", "Choose whether to make this text strikethrough."), dynamicTextData.strikethrough);

        if (dynamicTextData.colours != null)
        {
            if (dynamicTextData.colours.Length > 1)
            {
                dynamicTextData.numberOfColourAlternations = EditorGUILayout.IntField(new GUIContent("Number of colour alternations: ", "The number of times the colour will alternate."),
                    dynamicTextData.numberOfColourAlternations);

                if (dynamicTextData.numberOfColourAlternations > 0)
                {
                    dynamicTextData.colourAlternationDuration = EditorGUILayout.FloatField(new GUIContent("Colour Alternation Duration: ",
                        "The duration in seconds between alternations in colour."), dynamicTextData.colourAlternationDuration);

                    EditorGUILayout.PropertyField(colourAlternationMode, new GUIContent("Colour Alternation Mode: ", "The alternation mode for colour. Immediate changes immediately between colours," +
                        " while gradient smoothly transitions between them."));
                }
            }
        }

        if (dynamicTextData.sizes != null)
        {
            if (dynamicTextData.sizes.Length > 1 && dynamicTextData.sizes != null)
            {
                dynamicTextData.numberOfSizeAlternations = EditorGUILayout.IntField(new GUIContent("Number of size alternations: ", "The number of times the size will alternate."),
                    dynamicTextData.numberOfSizeAlternations);

                if (dynamicTextData.numberOfSizeAlternations > 0)
                {
                    dynamicTextData.sizeAlternationDuration = EditorGUILayout.FloatField(new GUIContent("Size Alternation Duration: ",
                        "The duration in seconds between alternations in size."), dynamicTextData.sizeAlternationDuration);

                    EditorGUILayout.PropertyField(sizeAlternationMode, new GUIContent("Size Alternation Mode: ", "The alternation mode for size. Immediate changes immediately between sizes," +
                        " while gradient smoothly transitions between them."));
                }
            }
        }

        dynamicTextData.enterDuration = EditorGUILayout.FloatField(new GUIContent("Enter Duration: ", "The time in seconds for which entry animations play."), dynamicTextData.enterDuration);

        dynamicTextData.exitDuration = EditorGUILayout.FloatField(new GUIContent("Exit Duration: ", "The time in seconds for which exit animations play."), dynamicTextData.exitDuration);

        EditorGUILayout.PropertyField(enterType, new GUIContent("Enter Type: ", "An array storing the entry types for this text. Definitions can be found in the documentation, while examples" +
            " can be viewed on the demonstration video on the asset store."));

        EditorGUILayout.PropertyField(exitType, new GUIContent("Exit Type: ", "The exit type for this text. Definitions can be found in the documentation, while examples" +
            " can be viewed on the demonstration video on the asset store."));

        shiftOrBounceFound = false;
        popFound = false;
        if (dynamicTextData.enterType != null)
        {
            for (int i = 0; i < dynamicTextData.enterType.Length; i++)
            {
                if (dynamicTextData.enterType[i] == EnterType.Shift || dynamicTextData.enterType[i] == EnterType.Bounce)
                {

                    shiftOrBounceFound = true;

                    if (dynamicTextData.enterType[i] == EnterType.Bounce)
                    {
                        dynamicTextData.maxDrift = EditorGUILayout.FloatField(new GUIContent("Max Drift: ", "The height in metres which the text travels horizontally while entering."), dynamicTextData.maxDrift);
                    }
                }
                if (dynamicTextData.enterType[i] == EnterType.Pop)
                {
                    popFound = true;
                }
            }
        }
        if(dynamicTextData.exitType == ExitType.Pop)
        {
            popFound = true;
        }
        if (shiftOrBounceFound)
        {
            dynamicTextData.maxHeight = EditorGUILayout.FloatField(new GUIContent("Max Height: ", "The height in metres which the text travels vertically while entering."), dynamicTextData.maxHeight);
        }
        if (popFound)
        {
            dynamicTextData.popModifier = EditorGUILayout.FloatField(new GUIContent("Pop Modifier: ", "The scale, as a modifier of the original scale, " +
                        " to which the text will pop while entering and/or exiting."), dynamicTextData.popModifier);
        }

        if (GUILayout.Button("Save Data"))
        {
            EditorUtility.SetDirty(target);
            Debug.Log("Text Data saved successfully.");
        }

        serializedObject.ApplyModifiedProperties();

    }

}
