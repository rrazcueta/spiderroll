using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemData itemData;
    public int count;

    public Item(ItemData itemData, int count)
    {
        this.itemData = itemData;
        this.count = count;
    }

    override public string ToString()
    {
        if (IsEmpty())
            return "Empty";

        if (count > 1)
            return itemData.itemName + " x" + count;

        return itemData.itemName;
    }

    public bool IsEmpty()
    {
        return itemData == null || count == 0;
    }
}
