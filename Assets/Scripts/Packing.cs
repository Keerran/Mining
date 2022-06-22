using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// The Paki™ algorithm
public class Packing
{
    public int xSize;
    public int ySize;
    public Vector2[] objects;
    public List<(Vector2, int)> positions = new List<(Vector2, int)>();

    // Array of vector 4's which indicate space
    // around cell w/ 2 'vector2's prioritising xdir then ydir
    private Vector4[][] _sizes;

    public Packing(int xSize, int ySize, Vector2[] objects)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.objects = objects;

        _sizes = new Vector4[xSize][];
        for(int i = 0; i < xSize; i++)
        {
            _sizes[i] = new Vector4[ySize];
            for(int j = 0; j < ySize; j++)
            {
                _sizes[i][j] = new Vector4(xSize - i, ySize - j, xSize - i, ySize - j);
            }
        }
    }

    (Vector2?, int) PlaceObject()
    {
        var i = Random.Range(0, objects.Length);
        var obj = objects[i];
        var positions = new List<Vector2>();
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                var size = _sizes[x][y];
                
                if((obj.x <= size.x && obj.y <= size.y) || (obj.x <= size.z && obj.y <= size.w))
                {
                    positions.Add(new Vector2(x, y));
                }
            }
        }

        if(positions.Count == 0)
            return (null, i);

        return (positions[Random.Range(0, positions.Count)], i); 
    }

    // TODO Use int vector2 instead of float vector2
    void Collapse(Vector2 pos, Vector2 size)
    {
        int x = (int)(pos.x + size.x - 1), y = (int)(pos.y + size.y - 1);
        int dx = 0;
        for(int i = x; i >= 0; i--)
        {
            int dy = 0;
            if(i < pos.x)
            {
                dx++;
            }
            for(int j = y; j >= 0; j--)
            {
                if(i >= pos.x && j >= pos.y)
                {
                    _sizes[i][j] = Vector4.zero;
                    continue;
                }
                if(j < pos.y)
                {
                    dy++;
                }

                var max = _sizes[i][j];

                if(i < pos.x && j < pos.y)
                {
                    if(dx <= max.z)
                    {
                        _sizes[i][j].z = dx;
                    }
                    if(dy <= max.y)
                    {
                        _sizes[i][j].y = dy;
                    }
                }
                else if(i < pos.x)
                {
                    if(dx <= max.x)
                    {
                        _sizes[i][j].x = dx;
                        if(max.x != 0)
                            _sizes[i][j].y = Mathf.Min(max.y, _sizes[i+1][j].y);
                    }
                    if(dx <= max.z)
                    {
                        _sizes[i][j].z = dx;
                    }
                }
                else if(j < pos.y)
                {
                    if(dy <= max.w)
                    {
                        _sizes[i][j].w = dy;
                        if(max.x != 0)
                            _sizes[i][j].z = Mathf.Min(max.z, _sizes[i][j+1].z);
                    }
                    if(dy <= max.y)
                    {
                        _sizes[i][j].y = dy;
                    }
                }
            }
        }
    }

    public bool Place()
    {
        var (position, obj) = PlaceObject();

        if(position is Vector2 pos)
        {
            positions.Add((pos, obj));
            Collapse(pos, objects[obj]);
            return true;
        }
        return false;
    }

    public void Log()
    {
        var sb = new StringBuilder();

        for(int j = 0; j < ySize; j++)
        {
            for(int i = 0; i < xSize; i++)
            {
                var max = _sizes[i][j];
                sb.Append($"{max.x}|{max.y}|{max.z}|{max.w},");
            }
            sb.Append("\n");
        }

        Debug.Log(sb.ToString());
    }
}