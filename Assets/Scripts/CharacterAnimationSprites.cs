using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSprites", menuName = "ScriptableObjects/Character Sprites", order = 1)]
public class CharacterAnimationSprites : ScriptableObject
{
    public Sprite[] sprites;
    public Sprite[] backSprites;
}
