using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public List<GameObject> playerObjects = new List<GameObject>();
    public List<GameObject> injuredPlayerObjects = new List<GameObject>();
    public List<GameObject> healthyPlayerObjects = new List<GameObject>();

    public GameObject player;
    public int playerCount;

    void Awake()
    {
        if (!GM)
        {
            GM = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    static public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Spawn(Transform[] spawnPoints)
    {
        int index = 0;

        for (int i = 0; i < playerCount; i++)
        {
            bool useDefault = spawnPoints == null || spawnPoints.Length == 0;

            while (!useDefault && index > spawnPoints.Length)
                index -= spawnPoints.Length;

            SpawnPlayer(useDefault ? Vector3.zero : spawnPoints[index].position);
            index++;
        }
    }

    void SpawnPlayer(Vector3 position)
    {
        GameObject newPlayer = Instantiate(player, position, Quaternion.identity);

        playerObjects.Add(newPlayer);
        healthyPlayerObjects.Add(newPlayer);
    }

    public static void PlayerIsInjured(GameObject player)
    {
        if (GM.injuredPlayerObjects.Contains(player))
        {
            Debug.Log("Player is already in the injuredPlayerObjects list: " + player);
            return;
        }

        GM.injuredPlayerObjects.Add(player);
        GM.healthyPlayerObjects.Remove(player);

        GM.CheckGameEnd();
    }

    public static void PlayerHasRecovered(GameObject player)
    {
        if (!GM.injuredPlayerObjects.Contains(player))
        {
            Debug.Log("Unknown player to injuredPlayerObjects list: " + player);
            return;
        }

        GM.injuredPlayerObjects.Remove(player);
        GM.healthyPlayerObjects.Add(player);
    }

    void CheckGameEnd()
    {
        if (playerObjects.Count == injuredPlayerObjects.Count)
        {
            Debug.Log("END GAME");
            LevelManager.SET_TEXT("GAME OVER");
            LevelManager.SET_RESET_TIMER(5);
        }
    }

    internal static void RESET()
    {
        LevelManager.RESET_WHEN = float.MaxValue;
        SceneManager.LoadScene("Title");
        GM.playerObjects = new List<GameObject>();
        GM.injuredPlayerObjects = new List<GameObject>();
        GM.healthyPlayerObjects = new List<GameObject>();

        PlayerController.RESET();
    }
}
