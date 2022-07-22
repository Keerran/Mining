using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static FastNoiseLite;

public class TerrainGenerator : EditorWindow
{
    public GameObject terrain;
    [Range(0, 20)]
    public float scale;
    [Range(0, 20)]
    public int layers;
    [Range(0, 1)]
    public float persistence;
    [Range(1, 10)]
    public float multiplier;
    [Range(0, 2)]
    public float amplitude;
    public bool invert;

    private SerializedObject _serialized;
    private Texture2D _noiseTex;
    private FastNoiseLite _noise;

    [MenuItem("Window/Terrain Generator")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<TerrainGenerator>();
        window.Show();
    }

    void OnEnable()
    {
        _serialized = new SerializedObject(this);
        _noise = new FastNoiseLite();
        _noise.SetNoiseType(NoiseType.OpenSimplex2);
        CreateNoise();
    }

    void CreateField(string property)
    {
        EditorGUILayout.PropertyField(_serialized.FindProperty(property));
    }

    float GetNoise(float x, float y)
    {
        return 0.5f * _noise.GetNoise(x, y) + 0.5f;
    }

    float LayerNoise(float x, float y)
    {
        var factor = 1f;
        var f = 0f;
        var s = scale;
        for(int i = 0; i < layers; i++)
        {
            var offset = i * _noiseTex.width;
            f += GetNoise(x * s + offset, y * s + offset) * factor;
            factor *= persistence;
            s *= multiplier;
        }

        return f;
    }

    float LayerNoise(Vector2 pos)
    {
        return LayerNoise(pos.x, pos.y);
    }

    void CreateNoise()
    {
        if(_noiseTex)
            DestroyImmediate(_noiseTex);
        _noiseTex = new Texture2D(100, 100);
        for(int x = 0; x < 100; x++)
        {
            for(int y = 0; y < 100; y++)
            {
                var f = LayerNoise(x, y);
                _noiseTex.SetPixel(x, y, new Color(f, f, f, 1));
            }
        }
        _noiseTex.Apply();
    }

    void Generate()
    {
        var mesh = terrain.GetComponent<MeshFilter>().sharedMesh;
        var vertices = mesh.vertices;
        var bounds = mesh.bounds;
        foreach(var (i, v) in vertices.Enumerate())
        {
            var f = LayerNoise(50f * v.XZ() + 50f * Vector2.one);
            vertices[i] = v + Vector3.up * amplitude * f * (invert ? -1 : 1);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    void OnGUI()
    {
        _serialized.Update();

        CreateField("terrain");
        CreateField("scale");
        CreateField("layers");
        CreateField("persistence");
        CreateField("multiplier");

        var rect = EditorGUILayout.GetControlRect(false, 120);
        rect.x = rect.width / 2 - 50;
        rect.y += 10;
        rect.width = 100;
        rect.height = 100;
        EditorGUI.DrawPreviewTexture(rect, _noiseTex);

        CreateField("amplitude");
        CreateField("invert");

        if(GUILayout.Button("Generate"))
            Generate();

        if(_serialized.hasModifiedProperties)
        {
            _serialized.ApplyModifiedProperties();
            CreateNoise();
        }
    }
}
