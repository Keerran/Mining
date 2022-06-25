using System.Collections;
using System.Collections.Generic;
using Serializeable;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "MiningTool")]
public class MiningTool : ScriptableObject
{
    public string type;
    public IntMatrix aoe = new IntMatrix(3, 3);


    [CustomEditor(typeof(MiningTool))]
    public class Editor : UnityEditor.Editor
    {
        private MiningTool script;

        void OnEnable()
        {
            script = (MiningTool)target;
        } 

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            for(int i = 0; i < script.aoe.GetLength(0); i++)
            {
                EditorGUILayout.BeginHorizontal();
                for(int j = 0; j < script.aoe.GetLength(1); j++)
                {
                    script.aoe[i][j] = EditorGUILayout.IntField(script.aoe[i][j]);
                }
                EditorGUILayout.EndHorizontal();
            }
            var changed = EditorGUI.EndChangeCheck();
            if(changed)
            {
                EditorUtility.SetDirty(script);
                // AssetDatabase.Refresh();
            }
        }
    }

}
