using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TitleMenu : MonoBehaviour
{
    Player playerOne;
    bool left;
    bool right;
    bool leftDown;
    bool rightDown;
    bool confirm;
    bool confirmDown;

    TMP_Text menu;
    public string[] menuOptions;
    int optionsIndex;

    void Awake()
    {
        playerOne = ReInput.players.GetPlayer(0);
        menu = GetComponent<TMP_Text>();

        if (GameManager.GM)
            optionsIndex = GameManager.GM.playerCount - 1;
    }

    void Update()
    {
        GetInput();

        if (leftDown)
        {
            optionsIndex--;
        }
        if (rightDown)
        {
            optionsIndex++;
        }
        if (optionsIndex < 0)
        {
            optionsIndex = menuOptions.Length - 1;
        }
        if (optionsIndex >= menuOptions.Length)
        {
            optionsIndex = 0;
        }

        menu.text = menuOptions[optionsIndex];

        if (confirmDown)
            Select();
    }

    void Select()
    {
        int playerCount = optionsIndex + 1;

        GameManager.GM.playerCount = playerCount;

        if (optionsIndex != 8)
            GameManager.LoadScene("Test");
    }

    void GetInput()
    {
        bool jumpThisFrame = playerOne.GetButton("Jump");
        confirmDown = !confirm && jumpThisFrame;
        confirm = jumpThisFrame;

        bool leftThisFrame = playerOne.GetAxisRaw("Horizontal") < 0;
        leftDown = !left && leftThisFrame;
        left = leftThisFrame;

        bool rightThisFrame = playerOne.GetAxisRaw("Horizontal") > 0;
        rightDown = !right && rightThisFrame;
        right = rightThisFrame;
    }
}
