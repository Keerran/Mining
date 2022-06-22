using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mining : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public GameObject[] prefabs;
    public MineItem[] items;
    public List<InventoryItem> drops;

    private int[][] _board;
    private Camera _camera;
    private (Vector2, int)[] _mineItems;
    private int[] _covered;

    // Start is called before the first frame update
    void Awake()
    {
        _camera = Camera.main;
        _board = new int[xSize][];
        for(int i = 0; i < xSize; i++)
        {
            _board[i] = new int[ySize];
            for(int j = 0; j < ySize; j++)
            {
                _board[i][j] = Random.Range(0, 3);
                CreateCell(i, j);
            }
        }

        var model = new Packing(xSize, ySize, items.Select(it => it.size).ToArray());

        for(int i = 0; i < 5; i++)
            model.Place();

        foreach(var (place, name) in model.positions)
        {
            var pos = new Vector3(place.x - (xSize / 2) - 0.5f, -place.y + (ySize / 2) - 0.5f, 0.1f);
            var go = Instantiate(items[name].prefab, pos, Quaternion.identity);
        }

        _covered = new int[model.positions.Count()];
        for(int i = 0; i < _covered.Length; i++)
        {
            var (pos, name) = model.positions[i];
            var rect = items[name].size;
            _covered[i] = (int)(rect.x * rect.y);
        }
        _mineItems = model.positions.ToArray();
    }

    void CreateCell(int i, int j)
    {
        var pos = new Vector3(i - (xSize / 2), j - (ySize / 2), 0);
        var go = Instantiate(prefabs[_board[i][j]], pos, Quaternion.identity);
        var cell = go.AddComponent(typeof(Cell)) as Cell;
        cell.x = i;
        cell.y = j;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                var cell = hit.collider.transform.parent.GetComponent<Cell>();
                if(cell != null)
                {
                    var (x, y) = cell;
                    var go = cell.gameObject;
                    if(_board[x][y] > 0)
                    {
                        _board[x][y]--;
                        CreateCell(x, y);
                    }
                    else
                    {
                        var vec = new Vector2(x, ySize - y - 1);
                        for(int i = 0; i < _covered.Length; i++)
                        {
                            var (pos, it) = _mineItems[i];
                            if(pos.LessThan(vec) && vec.LessThan(pos + items[it].size - Vector2.one))
                            {
                                if(--_covered[i] == 0)
                                {
                                    drops.Add(items[it].item);
                                    Inventory.instance.AddItem(items[it].item);
                                }
                                break;
                            }
                        }
                    }
                    Destroy(go);
                }
            }
        }
    }
}
