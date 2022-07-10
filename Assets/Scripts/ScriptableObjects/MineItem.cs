using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MineItem")]
public class MineItem : ScriptableObject
{
    public GameObject prefab;
    public Vector2 size;
    public InventoryItem item;
}
