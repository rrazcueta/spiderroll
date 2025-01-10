using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    static LevelManager LM;

    [SerializeField]
    Transform[] spawnPoints;

    public RectTransform healthBar;
    public TMP_Text text;

    public static float RESET_WHEN = float.MaxValue;

    void Awake()
    {
        if (!LM)
        {
            LM = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameManager.GM.Spawn(spawnPoints);
    }

    void Update()
    {
        if (RESET_WHEN < Time.time)
        {
            GameManager.RESET();
        }
    }

    public static void SET_HEALTH_BAR(float percent)
    {
        if (!LM)
            return;
        if (!LM.healthBar)
            return;

        LM.healthBar.gameObject.SetActive(percent >= 0);

        int width = (int)(400 * percent);
        LM.healthBar.sizeDelta = new Vector2(width, 10);
    }

    public static void SET_TEXT(string text)
    {
        if (!LM)
            return;
        if (!LM.text)
            return;

        LM.text.gameObject.SetActive(true);
        LM.text.text = text;
    }

    public static void SET_RESET_TIMER(float delay)
    {
        RESET_WHEN = Time.time + delay;
    }
}
