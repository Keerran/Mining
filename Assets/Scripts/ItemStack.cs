using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public InventoryItem item;
    public int size;

    public ItemStack(InventoryItem item, int size)
    {
        this.item = item;
        this.size = size;
    }

    public void Add(int amount = 1)
    {
        size += amount;
    }

    public void Remove(int amount = 1)
    {
        size = (int)Mathf.Max(size - amount, 0);
    }
}
