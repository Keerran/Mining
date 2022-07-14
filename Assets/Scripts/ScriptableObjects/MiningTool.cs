using System.Collections;
using System.Collections.Generic;
using Serializeable;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "MiningTool")]
public class MiningTool : ScriptableObject
{
    public string type;
    public Sprite buttonImage;
    public IntMatrix area = new IntMatrix(3, 3);
}
