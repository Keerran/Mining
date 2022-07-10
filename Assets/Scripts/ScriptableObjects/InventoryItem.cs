using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite image;
}
