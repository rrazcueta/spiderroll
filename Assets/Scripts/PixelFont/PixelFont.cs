using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelFont : MonoBehaviour
{
    public PixelFontSettings pixelFontSettings;

    /*
        Write a line text
        Sprites, positions for sprites
        Overall size for line
    */

    public string text;
    public bool blit;
    public float pixelSize = 0.1f;
    public float width;
    public float height;
    public enum Anchor { TopLeft, Top, TopRight, Left, Middle, Right, BottomLeft, Bottom, BottomRight }
    public Anchor anchor;

    public List<GameObject> characters;

    void Awake()
    {
        characters = new List<GameObject>();

        WriteText();
    }

    void LateUpdate()
    {
        if (blit)
        {
            blit = false;
            WriteText();
        }
    }

    void OnDisable(){
        ClearText();
    }

    void OnEnable(){
        WriteText();
    }

    void ClearText()
    {
        foreach (GameObject character in characters)
        {
            Destroy(character);
        }
        characters.Clear();

        width = 0;
        height = pixelFontSettings.defaultHeight;
    }

    void WriteText()
    {
        ClearText();

        // Vector3 currentPosition = transform.position;
        int currentPosition = 0;

        char[] charArr = text.ToCharArray();
        for (int i = 0; i < charArr.Length; i++)
        {
            int glyphIndex = (int)text[i] - 33;  //charArr[i] - '0'; //33 is the decimal representation of '!'
            int pixels = glyphIndex < 0 || glyphIndex >= pixelFontSettings.glyphs.Length ? pixelFontSettings.spaceWidth : WriteCharacter(i, glyphIndex, currentPosition);
            // currentPosition += Vector3.right * pixels * pixelSize;
            currentPosition += pixels;
            width += pixels;
        }

        for (int i = 0; i < characters.Count; i++)
        {
            Transform charTrans = characters[i].transform;

            if (anchor == Anchor.TopLeft || anchor == Anchor.Top || anchor == Anchor.TopRight)
                charTrans.localPosition -= Vector3.up * height * pixelSize;

            if (anchor == Anchor.Left || anchor == Anchor.Middle || anchor == Anchor.Right)
                charTrans.localPosition -= Vector3.up * height / 2 * pixelSize;

            if (anchor == Anchor.TopRight || anchor == Anchor.Right || anchor == Anchor.BottomRight)
                charTrans.localPosition -= Vector3.right * width * pixelSize;

            if (anchor == Anchor.Top || anchor == Anchor.Middle || anchor == Anchor.Bottom)
                charTrans.localPosition -= Vector3.right * width / 2 * pixelSize;
        }
    }

    int WriteCharacter(int index, int glyphIndex, int position)
    {
        // GameObject charObj = Instantiate(new GameObject("Character [" + (char)(glyphIndex + 33) +  "] at " + index), position + Camera.main.gameObject.transform.forward * -0.1f, transform.rotation);

        GameObject charObj = new GameObject("Char [" + (char)(glyphIndex + 33) + "] at " + index);
        charObj.transform.position = transform.position; //position;
        charObj.transform.rotation = transform.rotation;
        charObj.transform.parent = transform;
        charObj.transform.localScale = Vector3.one;
        charObj.transform.localPosition = new Vector3(position * pixelSize, 0, 0);
        characters.Add(charObj);
        charObj.layer = LayerMask.NameToLayer("UI");

        SpriteRenderer sr = charObj.AddComponent<SpriteRenderer>();
        sr.sprite = pixelFontSettings.glyphs[glyphIndex];
        sr.sortingOrder = 10;

        return pixelFontSettings.defaultWidth + pixelFontSettings.widthAdjustments[glyphIndex];
    }
}
