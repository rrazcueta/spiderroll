using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct Effect
    {
        public string name;
        public GameObject effect;
    }

    [SerializeField]
    Effect[] effects;

    Dictionary<string, GameObject> effectsDictionary;

    void Awake()
    {
        effectsDictionary = new Dictionary<string, GameObject>();
        foreach (Effect e in effects)
        {
            effectsDictionary.Add(e.name, e.effect);
        }
    }

    public GameObject Spawn(string key, Vector3 relativePosition, Quaternion rotation)
    {
        GameObject toSpawn = effectsDictionary[key];
        if (!toSpawn)
            return null;
        return Instantiate(toSpawn, transform.position + relativePosition, rotation);
    }
}
