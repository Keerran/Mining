using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public class SaveData
{
    [Key(0)]
    public Dictionary<string, int> itemIds;
}
