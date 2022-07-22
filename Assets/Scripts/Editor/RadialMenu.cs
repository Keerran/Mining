
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadialMenu : EditorWindow
{
    public Transform canvas;
    public GameObject buttonPrefab;
    public float radius;

    private SerializedObject _serialized;

    [MenuItem("Window/Radial Menu")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<RadialMenu>();
        window.Show();
    }

    void Create()
    {
#if UNITY_EDITOR
        var mining = FindObjectOfType<Mining>();
        var menu = new GameObject();
        menu.AddComponent<RectTransform>().SetParent(canvas, false);
        menu.name = "ToolWheel";
        foreach (var (i, tool) in mining.tools.Enumerate())
        {

            var rot = Quaternion.Euler(0, 0, 360 * (float)i / mining.tools.Length);

            var button = Instantiate(buttonPrefab, menu.transform);
            button.name = $"{tool.name}Button";
            button.transform.localRotation = rot;
            UnityEventTools.AddObjectPersistentListener<MiningTool>(
                button.GetComponent<Button>().onClick,
                new UnityAction<MiningTool>(mining.ChangeTool),
                tool
            );
            button.GetComponent<ToolButton>().tool = tool;
            var buttonImage = button.GetComponent<Image>();

            buttonImage.fillAmount = 1f / mining.tools.Length;

            var image = button.transform.GetChild(0);
            var rect = image.GetComponent<RectTransform>();

            var theta = -Mathf.PI * ((1f) / mining.tools.Length - 0.5f);
            var pos = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            pos *= radius;
            rect.localPosition = pos;
            rect.localRotation = Quaternion.Inverse(rot);
            image.GetComponent<Image>().sprite = tool.buttonImage;
        }
#endif
    }

    void OnEnable()
    {
        _serialized = new SerializedObject(this);
    }

    void CreateField(string property)
    {
        EditorGUILayout.PropertyField(_serialized.FindProperty(property));
    }

    void OnGUI()
    {
        _serialized.Update();

        CreateField("canvas");
        CreateField("buttonPrefab");
        CreateField("radius");

        if (GUILayout.Button("Create"))
            Create();

        _serialized.ApplyModifiedProperties();
    }
}
