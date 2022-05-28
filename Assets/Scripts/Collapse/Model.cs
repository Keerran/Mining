using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using Random = System.Random;

public class Model
{
    public string[] tiles;

    Stack<(int, int)> stack = new Stack<(int, int)>();
    Random random = new Random();
    int xDim;
    int yDim;
    (int, int)[] directions = new [] {
        (0, -1),
        (0, 1),
        (-1, 0),
        (1, 0)
    };

    double[] weights;
    int[][] propagator;
    Dictionary<string, int> indices;
    bool[][] cells;
    int[][][] compatible;

    public Model(int xDim, int yDim, TextAsset xmlFile)
    {
        this.xDim = xDim;
        this.yDim = yDim;
        var doc = new XmlDocument();
        var root = XDocument.Load(new StringReader(xmlFile.text)).Root;
        var tileEls = root.Element("Modules")
                          .Elements("Module");

        tiles = tileEls.Select(tag => tag.Get("name"))
                       .ToArray();
        
        weights = tileEls.Select(tag => double.Parse(tag.Get("weight") ?? "1.0")).ToArray();

        var numTiles = tiles.Length;

        indices = tiles.Select((v, i) => new { Key = v, Value = i })
                       .ToDictionary(o => o.Key, o => o.Value);
        var adjacency = new bool[numTiles, numTiles];

        foreach(var rule in root.Element("Rules").Elements("Rule"))
        {
            var left = indices[rule.Get("left")];
            var right = indices[rule.Get("right")];
            adjacency[left, right] = true;
            adjacency[right, left] = true;
        }

        propagator = new int[numTiles][];
        for(int i = 0; i < adjacency.GetLength(0); i++)
        {
            var adjIndices = new List<int>();
            for(int j = 0; j < adjacency.GetLength(1); j++)
            {
                if(adjacency[i, j])
                    adjIndices.Add(j);
            }
            propagator[i] = adjIndices.ToArray();
        }

        cells = new bool[xDim * yDim][];
        compatible = new int[cells.Length][][];
        for(int i = 0; i < compatible.Length; i++)
        {
            compatible[i] = new int[numTiles][];
            for(int j = 0; j < numTiles; j++)
                compatible[i][j] = new int[directions.Length];
        }
        Clear();
    }

    void Clear()
    {
        for(int cell = 0; cell < cells.Length; cell++)
        {
            cells[cell] = Enumerable.Repeat(true, tiles.Length).ToArray();
            for(int tile = 0; tile < tiles.Length; tile++)
                for(int dir = 0; dir < directions.Length; dir++)
                    compatible[cell][tile][dir] = propagator[tile].Length;
        }
    }

    public bool Run()
    {
        Clear();
        while(Step());

        return true;
    }

    public bool[][] Grid()
    {
        // var output = new string[xDim][];
        // for(int i = 0; i < xDim; i++)
        // {
        //     output[i] = new string[yDim];
        // }

        // for(int cell = 0; cell < cells.Length; cell++)
        // {
        //     int x = cell % xDim;
        //     int y = cell / xDim;
        //     if(cells[cell].Sum() == 1)
        //         output[x][y] = tiles[Array.IndexOf(cells[cell], true)];
        // }

        // return output;
        return cells;
    }

    public bool Step()
    {
        var x = FindCell();
        if(x != -1)
        {
            Observe(x);
            Propagate();
            return true;
        }

        return false;
    }

    void Ban(int cell, int tile)
    {
        cells[cell][tile] = false;

        int[] comp = compatible[cell][tile];
        for (int dir = 0; dir < directions.Length; dir++)
            comp[dir] = 0;

        stack.Push((cell, tile));
    }

    int FindCell()
    {
        double min = double.PositiveInfinity;
        var index = -1;
        for (int i = 0; i < cells.Length; i++)
        {
            var entropy = cells[i].Sum();
            if(entropy <= 1)
                continue;

            var noise = 1E-6 * random.NextDouble();
            if(entropy - noise < min) {
                min = entropy;
                index = i;
            }
        }

        return index;
    }

    int Observe(int cell)
    {
        double[] distribution = cells[cell].Select((v, i) => v ? weights[i] : 0.0).ToArray();

        var observation = distribution.Random(random.NextDouble());

        for(int tile = 0; tile < tiles.Length; tile++)
        {
            if(tile != observation && cells[cell][tile])
                Ban(cell, tile);
            }

        return observation;
    }

    void Propagate()
    {
        while(stack.Count > 0)
        {
            var (cell, tile) = stack.Pop();
            int x1 = cell % xDim;
            int y1 = cell / xDim;

            for(int dir = 0; dir < directions.Length; dir++)
            {
                var (offX, offY) = directions[dir];
                int x2 = x1 + offX;
                int y2 = y1 + offY;

                if(x2 < 0 || y2 < 0 || x2 >= xDim || y2 >= yDim)
                    continue;

                int cell2 = y2 * xDim + x2;

                foreach(var tile2 in propagator[tile])
                {
                    var compat = --compatible[cell2][tile2][dir];

                    if(compat == 0)
                    {
                        // Debug.Log($"{x1} {y1} {tiles[tile]} {x2} {y2} {tiles[tile2]} {compat}");
                        Ban(cell2, tile2);
                    }
                }
            }
        }
    }
}
// deez
// kenya
//ligma