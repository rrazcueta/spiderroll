using UnityEngine;

public class SpriteDirectionChooser : MonoBehaviour
{
    [SerializeField]
    Transform billboardedTransform;
    [SerializeField]
    Sprite[] sprites;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    bool flipIt;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void LateUpdate(){
        if(sprites.Length % 2 == 0) return;
        if(sprites.Length < 3) return;

        float direction = billboardedTransform.localEulerAngles.y;
        direction = (direction % 360 + 360) % 360;

        int directionCount = (sprites.Length-1) *2;
        float degreeIncrement = 360 / directionCount;
        float halfIncrement = degreeIncrement/2;

        flipIt = direction > 180 + halfIncrement;

        transform.localScale =  new Vector3(flipIt ? 1 : -1, 1, 1);

        for(int i = 0; i < directionCount; i++){
            float min = i * degreeIncrement - halfIncrement;
            float max = i * degreeIncrement + halfIncrement;
            if(direction >= min && direction <= max){
                // spriteRenderer.sprite = sprites[i];
                /*
                0 => up
                1 => next angle
                ... => so on and so forth
                if >= half directionCount down
                next previous angle
                .... => up
                */

                if(i <= directionCount/2){
                    spriteRenderer.sprite = sprites[i];
                    return;
                } else {
                    spriteRenderer.sprite = sprites[directionCount - i];
                    return;
                }

                spriteRenderer.sprite = sprites[0];
            }
        }
    }
}

/*
0
17
26
35
4
*/