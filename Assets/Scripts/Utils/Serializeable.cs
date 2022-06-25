using System;
using UnityEditor;
using UnityEngine;

namespace Serializeable
{
    [Serializable]
    public class IntArray
    {
        public int[] data;

        public IntArray(int size)
        {
            data = new int[size];
        }

        public static implicit operator int[](IntArray arr) => arr.data;
    }

    [Serializable]
    public class IntMatrix
    {
        public IntArray[] data;

        public IntMatrix(int xSize, int ySize)
        {
            data = new IntArray[xSize];
            for(int i = 0; i < xSize; i++)
                data[i] = new IntArray(ySize);
        }

        public int GetLength(int rank)
        {
            switch(rank)
            {
                case 0:
                    return data.Length;
                case 1:
                    return ((int[])data[0]).Length;
                default:
                    throw new ArgumentException($"Rank {rank} does not exist on matrix.");
            }
        }

        public int[] this[int x] => data[x];
    }

    [CustomPropertyDrawer(typeof(IntMatrix))]
    public class IntMatrixDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 3;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var data = property.FindPropertyRelative("data");

            var rows = data.arraySize;
            var height = rect.height / rows;

            for(int i = 0; i < rows; i++)
            {
                var row = data.GetArrayElementAtIndex(i).FindPropertyRelative("data");
                var cols = row.arraySize;
                var width = rect.width / cols;
                for(int j = 0; j < cols; j++)
                {
                    var pos = new Rect(
                        rect.x + width * j,
                        rect.y + height * i,
                        width,
                        height
                    );
                    EditorGUI.PropertyField(pos, row.GetArrayElementAtIndex(j), GUIContent.none);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}