using UnityEngine;

public class PaletteSwap : MonoBehaviour
{
    SpriteRenderer sr;
    Material material;
    public Vector3 rampTweak;
    public Color color1;//skin
    public Color shadow1;
    public Color color2;//hair
    public Color shadow2;
    public Color color3;//cloth
    public Color shadow3;
    public Color color4;//leather
    public Color shadow4;
    public Color color5;//metal
    public Color shadow5;

    // Start is called before the first frame update
    void Awake()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        material = sr.material;
    }

    void Update()
    {
        material.SetVector("_RampTweak", rampTweak);
        material.SetColor("_Color1", color1);
        material.SetColor("_Shadow1", shadow1);
        material.SetColor("_Color2", color2);
        material.SetColor("_Shadow2", shadow2);
        material.SetColor("_Color3", color3);
        material.SetColor("_Shadow3", shadow3);
        material.SetColor("_Color4", color4);
        material.SetColor("_Shadow4", shadow4);
        material.SetColor("_Color5", color5);
        material.SetColor("_Shadow5", shadow5);
    }
}
