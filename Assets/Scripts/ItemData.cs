using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemData : ScriptableObject
{
    public enum AmmoType {Arrow, Bolt, Rock};

    public string itemName;
    public float weight;
    public Sprite sprite;
    public bool usesAmmo;
    public bool isAmmo;
    public AmmoType ammoType;
}