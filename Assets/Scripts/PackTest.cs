using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackTest : MonoBehaviour
{
    public string[] names;
    public Vector2[] objects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [CustomEditor(typeof(PackTest))]
    public class Editor : UnityEditor.Editor
    {
        private PackTest script;

        void OnEnable()
        {
            script = (PackTest)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Run"))
            {
                var model = new Packing(10, 10, script.objects);
                model.Place();
            }
        }
    }
}
