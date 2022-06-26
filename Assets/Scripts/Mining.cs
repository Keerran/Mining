using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mining : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public GameObject empty;
    public GameObject[] prefabs;
    public MineItem[] items;
    public List<InventoryItem> drops;
    public MiningTool tool;

    private bool _unloading;
    private int[][] _board;
    private Camera _camera;
    private GameObject[][] _gameObjects;
    private (Vector2Int, int)[] _mineItems;
    private int[] _covered;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(gameObject.scene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _board = new int[xSize][];
        _gameObjects = new GameObject[xSize][];
        for(int i = 0; i < xSize; i++)
        {
            _board[i] = new int[ySize];
            _gameObjects[i] = new GameObject[ySize];
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
            var pos = new Vector3(place.x - (xSize / 2), -place.y + (ySize / 2), 0.1f);
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
        var prefab = _board[i][j] >= 0 ? prefabs[_board[i][j]] : empty;
        var pos = new Vector3(i - (xSize / 2) + 0.5f, j - (ySize / 2) + 0.5f, 0);
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var cell = go.AddComponent(typeof(Cell)) as Cell;
        cell.x = i;
        cell.y = j;
        _gameObjects[i][j] = go;
    }

    IEnumerator UnloadScene()
    {
        var unloadTask = SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
        while(!unloadTask.isDone)
            yield return null;
        var scene = SceneManager.GetActiveScene();
        foreach(var obj in scene.GetRootGameObjects())
            obj.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        yield return null;
    }

    void DropItem(InventoryItem item)
    {
        drops.Add(item);
        Inventory.instance.AddItem(item);
        if(drops.Count() == _mineItems.Length)
        {
            _unloading = true;
            CoroutineManager.instance.LaunchCoroutine(UnloadScene());
        }
    }

    void HitCell(int x, int y, int amount)
    {
        if(amount == 0)
            return;

        var go = _gameObjects[x][y];
        Destroy(go);
        _board[x][y] -= amount;
        CreateCell(x, y);
        if (_board[x][y] < 0)
        {
            var vec = new Vector2(x, ySize - y - 1);
            for (int i = 0; i < _covered.Length; i++)
            {
                var (pos, it) = _mineItems[i];
                if ((pos + Vector2.zero).LessThan(vec) && vec.LessThan(pos + items[it].size - Vector2.one))
                {
                    if (--_covered[i] == 0)
                    {
                        DropItem(items[it].item);
                    }
                    break;
                }
            }
        }
    }

    public void ChangeTool(MiningTool tool)
    {
        this.tool = tool;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameState.instance.paused) return;

        if(Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                var cell = hit.collider.transform.GetComponentInParent<Cell>();
                if(cell != null)
                {
                    var (x, y) = cell;
                    for(int i = -1; i <= 1; i++)
                    {
                        for(int j = -1; j <= 1; j++)
                        {
                            if(Utils.Between(x + i, 0, xSize) && Utils.Between(y + j, 0, ySize))
                            {
                                var amount = tool.area[i + 1][j + 1];
                                HitCell(x + i, y + j, amount);
                                if(_unloading)
                                    return;
                            }
                        }
                    }
                }
            }
        }
    }
}
