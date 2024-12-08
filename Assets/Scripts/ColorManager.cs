using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColorManager : ScriptableObject
{
    public Color lightBaseColor, darkBaseColor;

    public Color[] middleColors;
}
