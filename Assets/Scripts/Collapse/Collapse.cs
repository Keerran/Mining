using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Collapse : MonoBehaviour
{
    public int xDim, yDim;
    public Tile[] tiles;
    public GameObject grid;
    public GameObject cell;
    public TextAsset xml;
    public Model model;

    private Dictionary<string, GameObject> prefabs;

    void Update()
    {
        prefabs = new Dictionary<string, GameObject>();
        foreach(var tile in tiles)
        {
            prefabs[tile.name] = tile.prefab;
        }
    }

    void UpdateGrid()
    {
        while(grid.transform.childCount > 0)
        {
            DestroyImmediate(grid.transform.GetChild(0).gameObject);
        }
        var output = model.Grid();
        for(int i = 0; i < xDim; i++)
        {
            for(int j = 0; j < yDim; j++)
            {
                var cell = output[i * xDim + j];
                var size = (int)Mathf.Ceil(Mathf.Sqrt(cell.Sum()));
                var count = 0;
                for(int tile = 0; tile < cell.Length; tile++)
                {
                    if(!cell[tile])
                        continue;
                    var prefab = prefabs[model.tiles[tile]];
                    var pos = new Vector3(1f * j + (float)(count % size) / size, 1f * i + (float)(count / size) / size, 0);
                    // var pos = new Vector3(i, j, 0);
                    var go = Instantiate(prefab, pos, Quaternion.identity, grid.transform);
                    go.transform.localScale = new Vector3(1.0f / size, 1.0f / size, 0);
                    count++;
                }
            }
        }
    }

    public IEnumerator CreateModel()
    {
        model = new Model(xDim, yDim, xml);
        // for(int i = 0; i < 5000; i++)
        // {
        //     model.Run();
        //     try
        //     {
        //         var output = model.Grid();
        //         Debug.Log(output.Select(row => $"[{row.Join()}]").Join());
        //         break;
        //     }
        //     catch(IndexOutOfRangeException e)
        //     {

        //     }
        // }
        UpdateGrid();
        yield return null;
    }

    public IEnumerator Step()
    {
        if (model != null)
            model.Step();

        UpdateGrid();

        yield return null;
    }

    public IEnumerator Run()
    {
        if (model != null)
            model.Run();

        UpdateGrid();

        yield return null;
    }

    [Serializable]
    public class Tile
    {
        public string name;
        public GameObject prefab;
    }
}