using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Collapse))]
public class CollapseEditor : Editor
{
    private Collapse script;

    void OnEnable()
    {
        script = ((Collapse)target);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Create Model"))
        {
            script.StartCoroutine(script.CreateModel());
        }
        if (GUILayout.Button("Step"))
        {
            script.StartCoroutine(script.Step());
        }
        if (GUILayout.Button("Run"))
        {
            script.StartCoroutine(script.Run());
        }
    }
}