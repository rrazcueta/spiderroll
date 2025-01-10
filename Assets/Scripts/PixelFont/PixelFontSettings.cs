using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PixelFont", menuName = "ScriptableObjects/PixelFont", order = 1)]
public class PixelFontSettings : ScriptableObject
{
    public Sprite[] glyphs;
    public int defaultHeight;
    public int defaultWidth;
    public int spaceWidth;
    public int[] widthAdjustments = new int[94];
}