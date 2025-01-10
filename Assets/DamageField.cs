using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : MonoBehaviour
{
    void OnTriggerEnter(Collider co)
    {
        CharacterDamage characterDamage = co.gameObject.GetComponent<CharacterDamage>();
        if (characterDamage)
            characterDamage.SetInjured();
    }
}
