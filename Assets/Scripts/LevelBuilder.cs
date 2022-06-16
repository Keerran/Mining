using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public float cellScale;
    public int xSize, ySize;
    public TextAsset rules;
    public Tile[] tiles;

    // Start is called before the first frame update
    void Start()
    {
        var prefabs = new Dictionary<string, GameObject>();
        foreach(var tile in tiles)
        {
            prefabs[tile.name] = tile.prefab;
        }

        var model = new Model(xSize, ySize, rules);

        do
        {
        model.Run();
        }
        while(model.Grid().Sum(x => x.Sum()) == 0);

        var grid = new GameObject();
        var output = model.Grid();
        for(int i = 0; i < xSize; i++)
        {
            for(int j = 0; j < ySize; j++)
            {
                var cell = output[i * xSize + j];
                for(int tile = 0; tile < cell.Length; tile++)
                {
                    if(!cell[tile])
                        continue;
                    var (tileName, index) = model.baseTiles[tile];
                    var prefab = prefabs[tileName];
                    var pos = new Vector3(12 * j, 0, 12 * i);
                    var go = Instantiate(prefab, pos, Quaternion.identity, grid.transform);
                    go.name = $"{tileName} {tile - index}";
                    go.transform.eulerAngles += 90 * (tile - index) * Vector3.up;
                    go.transform.localScale = Vector3.one * cellScale;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class Tile
    {
        public string name;
        public GameObject prefab;
    }
}
