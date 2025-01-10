using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    SpriteRenderer sr;
    // Material material;
    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        // material = sr.material;
    }
}
