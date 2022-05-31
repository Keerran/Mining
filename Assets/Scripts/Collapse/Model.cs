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
    static int MAX_CARDINALITY = 4;
    public string[] tiles;
    public Dictionary<string, int> firstOccurences = new Dictionary<string, int>();
    public List<(string, int)> baseTiles = new List<(string, int)>();

    Stack<(int, int)> stack = new Stack<(int, int)>();
    Random random = new Random();
    int xDim;
    int yDim;
    (int, int)[] directions = new[] {
        (-1, 0),
        (0, 1),
        (1, 0),
        (0, -1),
    };

    double[] weights;
    int[][][] propagator;
    Dictionary<string, int> indices;
    bool[][] cells;
    int[][][] compatible;
    int numTiles;

    public Model(int xDim, int yDim, TextAsset xmlFile)
    {
        this.xDim = xDim;
        this.yDim = yDim;
        var doc = new XmlDocument();
        var root = XDocument.Load(new StringReader(xmlFile.text)).Root;

        BuildPropagator(root);

        cells = new bool[xDim * yDim][];
        compatible = new int[cells.Length][][];
        for (int i = 0; i < compatible.Length; i++)
        {
            compatible[i] = new int[numTiles][];
            for (int j = 0; j < numTiles; j++)
                compatible[i][j] = new int[directions.Length];
        }
        Clear();
    }

    void BuildPropagator(XElement root)
    {
        var tileEls = root.Element("Modules")
                          .Elements("Module").ToArray();

        tiles = tileEls.Select(tag => tag.Get("name"))
                       .ToArray();
        var tileWeights = tileEls.Select(tag => double.Parse(tag.Get("weight") ?? "1.0"))
                                 .ToArray();
        indices = tiles.Select((v, i) => new { Key = v, Value = i })
                       .ToDictionary(o => o.Key, o => o.Value);

        var action = new List<int[]>();
        var weightList = new List<double>();

        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tileEls[i];
            var symmetry = tile.Get("symmetry");
            Func<int, int> rotate;
            int cardinality;

            switch (symmetry)
            {
                case "L":
                    cardinality = 4;
                    rotate = i => (i + 1) % 4;
                    break;
                case "X":
                    cardinality = 1;
                    rotate = i => i;
                    break;
                case "T":
                    cardinality = 4;
                    rotate = i => (i + 1) % 4;
                    break;
                case "I":
                    cardinality = 2;
                    rotate = i => 1 - i;
                    break;
                default:
                    throw new Exception($"Unknown symmetry \"{symmetry}\".");
            }

            var firstOccurrence = action.Count;
            var name = tile.Get("name");
            firstOccurences[name] = firstOccurrence;
            for (int t = 0; t < cardinality; t++)
            {
                var map = new int[MAX_CARDINALITY];

                map[0] = t;
                for (int perm = 1; perm < 4; perm++)
                {
                    map[perm] = rotate(map[perm - 1]);
                }

                action.Add(map.Select(x => x + firstOccurrence).ToArray());
                baseTiles.Add((name, firstOccurrence));
                weightList.Add(tileWeights[i] / cardinality);
            }
        }
        weights = weightList.ToArray();
        numTiles = action.Count;
        var adjacency = new bool[directions.Length, numTiles, numTiles];

        foreach (var rule in root.Element("Rules").Elements("Rule"))
        {
            var left = rule.Get("left").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var right = rule.Get("right").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var leftRot = left.Length == 1 ? 0 : int.Parse(left[1]);
            var rightRot = right.Length == 1 ? 0 : int.Parse(right[1]);

            var l = action[firstOccurences[left[0]]][leftRot];
            var d = action[l][1];
            var r = action[firstOccurences[right[0]]][rightRot];
            var u = action[r][1];

            adjacency[0, r, l] = true;
            adjacency[0, action[l][2], action[r][2]] = true;

            adjacency[1, u, d] = true;
            adjacency[1, action[d][2], action[u][2]] = true;
        }

        for (int i = 0; i < numTiles; i++)
        {
            for (int j = 0; j < numTiles; j++)
            {
                adjacency[2, j, i] = adjacency[0, i, j];
                adjacency[3, j, i] = adjacency[1, i, j];
            }
        }

        propagator = new int[directions.Length][][];
        for (int i = 0; i < adjacency.GetLength(0); i++)
        {
            propagator[i] = new int[numTiles][];
            for (int j = 0; j < adjacency.GetLength(1); j++)
            {
                var adjIndices = new List<int>();
                for (int k = 0; k < adjacency.GetLength(2); k++)
                {
                    if (adjacency[i, j, k])
                        adjIndices.Add(k);
                }
                propagator[i][j] = adjIndices.ToArray();
            }
        }

    }

    void Clear()
    {
        for (int cell = 0; cell < cells.Length; cell++)
        {
            cells[cell] = Enumerable.Repeat(true, numTiles).ToArray();
            for (int tile = 0; tile < numTiles; tile++)
                for (int dir = 0; dir < directions.Length; dir++)
                {
                    var opp = (dir + 2) % 4;
                    compatible[cell][tile][dir] = propagator[opp][tile].Length;
                }
        }
    }

    public bool Run()
    {
        Clear();
        while (Step()) ;

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
        if (x != -1)
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
            if (entropy <= 1)
                continue;

            var noise = 1E-6 * random.NextDouble();
            if (entropy - noise < min)
            {
                min = entropy - noise;
                index = i;
            }
        }

        return index;
    }

    int Observe(int cell)
    {
        double[] distribution = cells[cell]
            .Select((v, i) => v ? weights[i] : 0.0).ToArray();

        var observation = distribution.Random(random.NextDouble());

        for (int tile = 0; tile < numTiles; tile++)
        {
            if (tile != observation && cells[cell][tile])
                Ban(cell, tile);
        }

        return observation;
    }

    void Propagate()
    {
        while (stack.Count > 0)
        {
            var (cell, tile) = stack.Pop();
            int x1 = cell % xDim;
            int y1 = cell / xDim;

            for (int dir = 0; dir < directions.Length; dir++)
            {
                var (offX, offY) = directions[dir];
                int x2 = x1 + offX;
                int y2 = y1 + offY;

                if (x2 < 0 || y2 < 0 || x2 >= xDim || y2 >= yDim)
                    continue;

                int cell2 = y2 * xDim + x2;

                foreach (var tile2 in propagator[dir][tile])
                {
                    var compat = --compatible[cell2][tile2][dir];

                    if (compat == 0)
                    {
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