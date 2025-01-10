using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDamage : MonoBehaviour
{
    public bool toggleInjured;
    public bool injured;

    Animator animator;
    PlayerController pc;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        pc = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (toggleInjured)
        {
            toggleInjured = false;

            injured = !injured;

            if (injured)
                SetInjured();
            else
                Recover();
        }
    }

    public void SetInjured()
    {
        if (injured)
            return;

        GameManager.PlayerIsInjured(gameObject);
        pc.enabled = false;

        animator.Play("Injured");
        animator.SetBool("Injured", true);
    }

    public void Recover()
    {
        GameManager.PlayerHasRecovered(gameObject);
        pc.enabled = true;

        animator.SetBool("Injured", false);
    }

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.tag != "Player")
            return;

        CharacterDamage otherCharacterDamage = co.gameObject.GetComponent<CharacterDamage>();
        if (otherCharacterDamage)
            otherCharacterDamage.Recover();
    }
}
