using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(menuName = "Dynamic Floating Text/New Text Data")]
[Serializable]
public class DynamicTextData : ScriptableObject
{

    public string id;

    public Color[] colours;
    public float[] sizes;
    public TMP_FontAsset font;

    public bool bold;
    public bool italic;
    public bool underline;
    public bool strikethrough;

    public float lifetime;
    public int numberOfColourAlternations;
    public int numberOfSizeAlternations;
    public float colourAlternationDuration;
    public float sizeAlternationDuration;
    public float enterDuration;
    public float exitDuration;
    public float maxDrift;
    public float maxHeight;
    public float popModifier;

    public AlternationMode colourAlternationMode;
    public AlternationMode sizeAlternationMode;

    public EnterType[] enterType;
    public ExitType exitType;

}
