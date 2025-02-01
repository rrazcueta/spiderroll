using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public bool randomizeHeadBody;
    public CharacterAnimationSprites[] allHeads;
    public CharacterAnimationSprites[] allBodies;
    public Transform billboarder;
    public Transform headBillboarder;
    SpriteRenderer headSR;
    SpriteRenderer bodySR;
    SpriteRenderer bodyFrontSR;
    public CharacterAnimationSprites head;
    public CharacterAnimationSprites body;
    PaletteSwap headPS;
    PaletteSwap bodyPS;
    PaletteSwap bodyFrontPS;
    public int spriteIndex;
    public float direction;
    public float headDirection;
    float lastHeadDirection;
    float lastDirection;
    public bool invertX;

    [System.Serializable]
    public enum AnimationSet
    {
        Down,
        DownSide,
        Side,
        UpSide,
        Up
    };

    public DirectedAnimationSet directedAnimationSet = new DirectedAnimationSet(
        AnimationSet.Up,
        false
    );
    public DirectedAnimationSet directedHeadAnimationSet = new DirectedAnimationSet(
        AnimationSet.Up,
        false
    );
    public AnimationSet animationSet = AnimationSet.Up;
    public AnimationSet headAnimationSet = AnimationSet.Up;
    public Vector3 innerRotation;
    public Vector3 headRotation;
    static int framesPerDirection = 16;
    public Vector3 rampTweak;
    public Color skinColor;
    public Color skinShadowColor;
    public Color hairColor;
    public Color hairShadowColor;
    public Color cloth;
    public Color clothShadow;
    public Color leather;
    public Color leatherShadow;
    public Color metal;
    public Color metalShadow;
    public bool randomizeRampTweak;
    public bool randomizeSkin;
    public bool randomizeHair;
    public bool randomizeCloth;
    public bool randomizeLeather;
    public bool randomizeMetal;
    public bool updateValues;

    [System.Serializable]
    public enum Animation
    {
        Idle,
        Walk,
        Startup,
        Action,
        Smash,
        Stab,
        Recover,
        InTheAir,
        Hurt,
        Injured
    };

    [SerializeField]
    private Animation currentAnim;
    public float animationProgress = 0;
    public float animationRate = 1;

    [System.Serializable]
    public struct DirectedAnimationSet
    {
        public AnimationSet set;
        public bool invertHorizontal;

        public DirectedAnimationSet(AnimationSet set, bool invertHorizontal)
        {
            this.set = set;
            this.invertHorizontal = invertHorizontal;
        }
    }

    void RandomizeHeadBody()
    {
        if (allHeads.Length > 0)
        {
            head = allHeads[(int)(Random.Range(0, 1f) * allHeads.Length)];
        }
        if (allBodies.Length > 0)
        {
            body = allBodies[(int)(Random.Range(0, 1f) * allBodies.Length)];
        }
    }

    void Awake()
    {
        RandomizeHeadBody();

        if (!billboarder)
            return;
        Transform billboarderSpriteContainer = billboarder.Find("Sprites");

        if (!headSR)
            headSR = billboarderSpriteContainer.Find("Head").GetComponent<SpriteRenderer>();
        if (!bodySR)
            bodySR = billboarderSpriteContainer.Find("Body").GetComponent<SpriteRenderer>();
        if (!bodyFrontSR)
            bodyFrontSR = billboarderSpriteContainer
                .Find("BodyFront")
                .GetComponent<SpriteRenderer>();

        if (!headPS)
            headPS = billboarderSpriteContainer.Find("Head").GetComponent<PaletteSwap>();
        if (!bodyPS)
            bodyPS = billboarderSpriteContainer.Find("Body").GetComponent<PaletteSwap>();
        if (!bodyFrontPS)
            bodyFrontPS = billboarderSpriteContainer.Find("BodyFront").GetComponent<PaletteSwap>();

        animationProgress = Random.Range(0f, 1f);
        animationRate = Random.Range(0f, 0.1f) + 1f;
    }

    int AnimationSetIndex(AnimationSet set)
    {
        switch (set)
        {
            case AnimationSet.Up:
                return 4;
            case AnimationSet.UpSide:
                return 3;
            case AnimationSet.Side:
                return 2;
            case AnimationSet.DownSide:
                return 1;
            default:
                return 0;
        }
    }

    private void StartStartup()
    {
        /*
        loading an animation
        - play animation X
        - load animation timing per startup, action, recovery, etc
        */
        currentAnim = Animation.Startup;
        SetSpriteAnimation(currentAnim);
    }

    private void StartAction()
    {
        currentAnim = Animation.Action;
        SetSpriteAnimation(currentAnim);
    }

    private void StartRecover()
    {
        currentAnim = Animation.Recover;
        SetSpriteAnimation(currentAnim);
    }

    private void SetSpriteAnimation(Animation a)
    {
        currentAnim = a;
    }

    private void NextAttackPhase() { }

    void UpdateValues()
    {
        //updateValues = false;

        if (headPS.rampTweak != rampTweak)
            headPS.rampTweak = bodyPS.rampTweak = bodyFrontPS.rampTweak = rampTweak;

        headPS.color3 = bodyPS.color3 = bodyFrontPS.color3 = cloth;
        headPS.shadow3 = bodyPS.shadow3 = bodyFrontPS.shadow3 = clothShadow;

        headPS.color2 = bodyPS.color2 = bodyFrontPS.color2 = hairColor;
        headPS.shadow2 = bodyPS.shadow2 = bodyFrontPS.shadow2 = hairShadowColor;

        headPS.color4 = bodyPS.color4 = bodyFrontPS.color4 = leather;
        headPS.shadow4 = bodyPS.shadow4 = bodyFrontPS.shadow4 = leatherShadow;

        headPS.color5 = bodyPS.color5 = bodyFrontPS.color5 = metal;
        headPS.shadow5 = bodyPS.shadow5 = bodyFrontPS.shadow5 = metalShadow;

        headPS.color1 = bodyPS.color1 = bodyFrontPS.color1 = skinColor;
        headPS.shadow1 = bodyPS.shadow1 = bodyFrontPS.shadow1 = skinShadowColor;
    }

    void RandomizeRampTweak()
    {
        randomizeRampTweak = false;

        rampTweak = new Vector3(
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f,
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f,
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f
        );

        if (rampTweak.sqrMagnitude > 0.36f)
            rampTweak = rampTweak.normalized * 0.6f;
        if (rampTweak.sqrMagnitude < 0.16)
            rampTweak = rampTweak.normalized * 0.4f;

        headPS.rampTweak = bodyPS.rampTweak = bodyFrontPS.rampTweak = rampTweak;
    }

    void RandomizeCloth()
    {
        randomizeCloth = false;

        float red = 0,
            blue = 0,
            green = 0;

        float sqrDiff = 0;
        int count = 0;
        while (sqrDiff < 0.5f && count < 25)
        {
            count++;
            if (count >= 25)
                Debug.Log("Too many cloth loops");
            red = Random.Range(0.4f, 1f);
            blue = Random.Range(0.4f, 1f);
            green = Random.Range(0.4f, 1f);

            sqrDiff =
                Mathf.Pow(Mathf.Abs(red - blue), 2)
                + Mathf.Pow(Mathf.Abs(red - green), 2)
                + Mathf.Pow(Mathf.Abs(blue - green), 2);
        }

        cloth = new Color(red, blue, green);

        Vector3 tweak = new Vector3(
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f,
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f,
            (Random.Range(0, 2) * 2 - 1) * Mathf.Pow(Random.Range(0f, 1f), 2) / 2 + 0.5f
        );

        if (tweak.sqrMagnitude > 0.75f)
            tweak = tweak.normalized * 0.75f;

        clothShadow = Color.white - cloth;
        clothShadow.r *= tweak.x;
        clothShadow.g *= tweak.y;
        clothShadow.b *= tweak.z;

        headPS.color3 = bodyPS.color3 = bodyFrontPS.color3 = cloth;
        headPS.shadow3 = bodyPS.shadow3 = bodyFrontPS.shadow3 = clothShadow;
    }

    void RandomizeHair()
    {
        randomizeHair = false;

        Color hair;
        Color hairShadow;

        float red = 0,
            blue = 0,
            green = 0;

        float sqrDiff = 0;
        int count = 0;
        while (sqrDiff < 0.5f && count < 10)
        {
            count++;
            if (count >= 10)
                Debug.Log("Too many loops");
            red = Random.Range(0.2f, 1);
            blue = Random.Range(0.2f, 1);
            green = Random.Range(0.2f, 1);

            sqrDiff =
                Mathf.Pow(Mathf.Abs(red - blue), 2)
                + Mathf.Pow(Mathf.Abs(red - green), 2)
                + Mathf.Pow(Mathf.Abs(blue - green), 2);
        }

        hair = new Color(red, blue, green);
        float minRGB = Mathf.Min(Mathf.Min(red, green), blue);
        hairShadow = new Color(
            Random.Range(0.1f, minRGB),
            Random.Range(0.1f, minRGB),
            Random.Range(0.1f, minRGB)
        );
        hairShadow = (hair + hairShadow * 3) / 4;

        headPS.color2 = bodyPS.color2 = bodyFrontPS.color2 = hair;
        headPS.shadow2 = bodyPS.shadow2 = bodyFrontPS.shadow2 = hairShadow;

        hairColor = hair;
        hairShadowColor = hairShadow;
    }

    void RandomizeLeather()
    {
        randomizeLeather = false;

        float red = 0,
            blue = 0,
            green = 0;

        float sqrDiff = 0;
        int count = 0;
        while (sqrDiff < 0.3f && count < 25)
        {
            count++;
            if (count >= 25)
                Debug.Log("Too many cloth loops");
            red = Random.Range(0.4f, 1f);
            blue = Random.Range(0.1f, red);
            green = Random.Range(0.2f, red);

            sqrDiff =
                Mathf.Pow(Mathf.Abs(red - blue), 2)
                + Mathf.Pow(Mathf.Abs(red - green), 2)
                + Mathf.Pow(Mathf.Abs(blue - green), 2);
        }

        leather = new Color(red, blue, green);
        float minRGB = Mathf.Min(Mathf.Min(red, green), blue);
        leatherShadow = new Color(
            Random.Range(0.3f, minRGB),
            Random.Range(0.1f, minRGB),
            Random.Range(0.1f, minRGB)
        );
        leatherShadow = (Color.white - leather + leatherShadow * 5) / 8;

        headPS.color4 = bodyPS.color4 = bodyFrontPS.color4 = leather;
        headPS.shadow4 = bodyPS.shadow4 = bodyFrontPS.shadow4 = leatherShadow;
    }

    void RandomizeMetal()
    {
        randomizeMetal = false;

        float red = 0,
            blue = 0,
            green = 0;

        red = Random.Range(0.4f, 1f);
        blue = Random.Range(0.6f, 1f);
        green = Random.Range(0.5f, 1f);

        metal = new Color(red, blue, green);
        metalShadow = new Color(
            Random.Range(0, 0.4f),
            Random.Range(0, 0.3f),
            Random.Range(0.2f, 0.5f)
        );
        metalShadow = (Color.white - metal + metalShadow * 5) / 8;

        headPS.color5 = bodyPS.color5 = bodyFrontPS.color5 = metal;
        headPS.shadow5 = bodyPS.shadow5 = bodyFrontPS.shadow5 = metalShadow;
    }

    void RandomizeSkin()
    {
        randomizeSkin = false;

        Color skin;
        Color skinShadow;

        float red = 0,
            blue = 0,
            green = 0;

        float sqrDiff = 0;
        int count = 0;
        while (sqrDiff < 0.06f && count < 10)
        {
            count++;
            if (count >= 10)
                Debug.Log("Too many loops");
            red = Random.Range(0.5f, 1);
            blue = Random.Range(0.4f, red);
            green = Random.Range(0.2f, red);

            sqrDiff =
                Mathf.Pow(Mathf.Abs(red - blue), 2)
                + Mathf.Pow(Mathf.Abs(red - green), 2)
                + Mathf.Pow(Mathf.Abs(blue - green), 2);
        }

        skin = new Color(red, blue, green);
        float minRGB = Mathf.Min(Mathf.Min(red, green), blue);
        skinShadow = new Color(
            Random.Range(0.3f, minRGB),
            Random.Range(0.2f, minRGB),
            Random.Range(0.1f, minRGB)
        );
        skinShadow = (Color.white - skin + skinShadow * 7) / 10;

        headPS.color1 = bodyPS.color1 = bodyFrontPS.color1 = skin;
        headPS.shadow1 = bodyPS.shadow1 = bodyFrontPS.shadow1 = skinShadow;

        skinColor = skin;
        skinShadowColor = skinShadow;
    }

    void Update()
    {
        if (randomizeHeadBody)
        {
            randomizeHeadBody = false;
            RandomizeHeadBody();
        }
        // if(randomizeRampTweak) RandomizeRampTweak();
        if (randomizeCloth)
            RandomizeCloth();
        if (randomizeHair)
            RandomizeHair();
        if (randomizeLeather)
            RandomizeLeather();
        if (randomizeMetal)
            RandomizeMetal();
        if (randomizeSkin)
            RandomizeSkin();
        if (updateValues)
            UpdateValues();
    }

    void LateUpdate()
    {
        animationProgress += animationRate * Time.deltaTime;
        if (animationProgress >= 1)
            animationProgress -= (int)animationProgress;

        switch (currentAnim)
        {
            case Animation.Idle:
                spriteIndex = 0 + (int)(animationProgress * 4);
                break;
            case Animation.Walk:
                spriteIndex = 4 + (int)(animationProgress * 4);
                break;
            case Animation.Startup:
                spriteIndex = 8;
                break;
            case Animation.Action:
                spriteIndex = 9;
                break;
            case Animation.Smash:
                spriteIndex = 10;
                break;
            case Animation.Stab:
                spriteIndex = 11;
                break;
            case Animation.Recover:
                spriteIndex = 12;
                break;
            case Animation.InTheAir:
                spriteIndex = 6;
                break;
            case Animation.Hurt:
                spriteIndex = 13;
                break;
            case Animation.Injured:
                spriteIndex = 0;
                break;
        }

        direction = billboarder.localEulerAngles.y;
        innerRotation = billboarder.localEulerAngles;
        headDirection = headBillboarder.localEulerAngles.y;
        headRotation = headBillboarder.localEulerAngles;

        bool redirect = Mathf.Abs(direction - lastDirection) > 0.1f;
        bool rotateHead = Mathf.Abs(headDirection - lastHeadDirection) > 0.1f || redirect;

        if (redirect)
        {
            lastDirection = direction;
            directedAnimationSet = ConvertAngleToDirectedAnimationSet(innerRotation);
        }
        if (rotateHead)
        {
            lastHeadDirection = headDirection;
            directedHeadAnimationSet = ConvertAngleToDirectedAnimationSet(headRotation);
        }

        bool dirLeft = direction > 180;
        billboarder.localScale = new Vector3(dirLeft ? 1 : -1, 1.414214f, 1);

        spriteIndex = Mathf.Clamp(spriteIndex, 0, framesPerDirection - 1);

        // if (head.sprites.Length != framesPerDirection * 5) return;
        // headSR.sprite = head.sprites[spriteIndex + AnimationSetIndex(headAnimationSet) * framesPerDirection];
        // headSR.flipX = dirLeft ? invertX : !invertX;

        if (head.sprites.Length != framesPerDirection * 5)
            return;
        headSR.sprite = head.sprites[
            spriteIndex + AnimationSetIndex(directedHeadAnimationSet.set) * framesPerDirection
        ];
        headSR.flipX = dirLeft
            ? directedHeadAnimationSet.invertHorizontal
            : !directedHeadAnimationSet.invertHorizontal;

        // if (body.backSprites.Length != framesPerDirection * 5) return;
        // bodySR.sprite = body.backSprites[spriteIndex + AnimationSetIndex(animationSet) * framesPerDirection];
        // bodySR.flipX = dirLeft ? invertX : !invertX;

        if (body.backSprites.Length != framesPerDirection * 5)
            return;
        bodySR.sprite = body.backSprites[
            spriteIndex + AnimationSetIndex(directedAnimationSet.set) * framesPerDirection
        ];
        bodySR.flipX = dirLeft
            ? directedAnimationSet.invertHorizontal
            : !directedAnimationSet.invertHorizontal;

        // if (body.sprites.Length != framesPerDirection * 5) return;
        // bodyFrontSR.sprite = body.sprites[spriteIndex + AnimationSetIndex(animationSet) * framesPerDirection];
        // bodyFrontSR.flipX = dirLeft ? invertX : !invertX;

        if (body.sprites.Length != framesPerDirection * 5)
            return;
        bodyFrontSR.sprite = body.sprites[
            spriteIndex + AnimationSetIndex(directedAnimationSet.set) * framesPerDirection
        ];
        bodyFrontSR.flipX = dirLeft
            ? directedAnimationSet.invertHorizontal
            : !directedAnimationSet.invertHorizontal;
    }

    DirectedAnimationSet ConvertAngleToDirectedAnimationSet(Vector3 rotation)
    {
        float angle = rotation.y;

        if (angle < 0)
            angle += 360;
        else if (angle >= 360)
            angle -= 360;

        AnimationSet set;
        bool invert;

        // 0 is up, rotate counter clockwise
        // < 22.5 = up
        // < 67.5 = upleft
        // < 112.5 = left
        // < 157.5 = downleft
        // < 202.5 = down
        // < 247.5 = downright
        // < 292.5 = right
        // < 337.5 = upright
        // else up

        switch (angle)
        {
            case float d when d < 22.5:
                set = AnimationSet.Up;
                invert = false;
                break;
            case float d when d < 67.5:
                set = AnimationSet.UpSide;
                invert = true;
                break;
            case float d when d < 112.5:
                set = AnimationSet.Side;
                invert = true;
                break;
            case float d when d < 157.5:
                set = AnimationSet.DownSide;
                invert = true;
                break;
            case float d when d < 202.5:
                set = AnimationSet.Down;
                invert = false;
                break;
            case float d when d < 247.5:
                set = AnimationSet.DownSide;
                invert = false;
                break;
            case float d when d < 292.5:
                set = AnimationSet.Side;
                invert = false;
                break;
            case float d when d < 337.5:
                set = AnimationSet.UpSide;
                invert = false;
                break;
            default:
                set = AnimationSet.Up;
                invert = false;
                break;
        }

        // if(rotation.z != 0) invert = !invert; //TODO fix invert

        return new DirectedAnimationSet(set, invert);
    }
}
