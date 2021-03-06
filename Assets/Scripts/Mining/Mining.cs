using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Mining : MonoBehaviour
{
    public int xSize;
    public int ySize;
    public GameObject empty;
    public GameObject[] prefabs;
    public MineItem[] items;
    public List<InventoryItem> drops;
    public MiningTool[] tools;
    public MiningTool tool;
    public GameObject spot;
    public Transform focusUI;

    private EventSystem _eventSystem;
    private bool _unloading;
    private int[][] _board;
    private Camera _camera;
    private GameObject[][] _gameObjects;
    private (Vector2Int, int)[] _mineItems;
    private int[] _covered;
    private Vector2Int _focus;
    private Vector2 _lastMousePos;

    void Awake()
    {
        var controls = StateManager.controls;
        controls.Mining.Move.performed += MoveFocus;
        controls.Mining.Mine.performed += Mine;
        controls.Mining.ToolWheel.performed += ToolWheel;

        _eventSystem = FindObjectOfType<EventSystem>();
    }

    void ToolWheel(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        if(input.magnitude <= 0.5)
            return;
        var angle = 0.5f - Vector2.SignedAngle(Vector2.down, input) / 360f;

        ChangeTool(tools[(int)(angle * tools.Length)]);
    }

    void MoveFocus(InputAction.CallbackContext ctx) {
        var move = Vector2Int.RoundToInt(ctx.ReadValue<Vector2>());

        if(move != Vector2Int.zero)
        {
            Vector2Int pos;
            if(_focus.x == -1)
                pos = new Vector2Int(xSize - 1, ySize - 1) * (Vector2Int.one - move) / 2;
            else
                pos = new Vector2Int(_focus.x + move.x, _focus.y + move.y);

            if(Utils.Between(pos.x, 0, xSize) && Utils.Between(pos.y, 0, ySize))
            {
                SetFocus(pos.x, pos.y);
            }
        }
    }

    void Mine(InputAction.CallbackContext ctx)
    {
        if(_focus.x != -1)
        {
            var (x, y) = (_focus.x, _focus.y);
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(gameObject.scene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        var controls = StateManager.controls;
        controls.Mining.Move.performed -= MoveFocus;
        controls.Mining.Mine.performed -= Mine;
    }

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
        _mineItems = model.positions.ToArray();
        for(int i = 0; i < _covered.Length; i++)
        {
            var (pos, name) = _mineItems[i];
            var rect = items[name].size;
            _covered[i] = (int)(rect.x * rect.y);
        }
    }

    Vector3 GetPosition(int i, int j)
    {
        return new Vector3(i - (xSize / 2) + 0.5f, j - (ySize / 2) + 0.5f, 0);
    }

    void CreateCell(int i, int j)
    {
        var prefab = _board[i][j] >= 0 ? prefabs[_board[i][j]] : empty;
        var pos = GetPosition(i, j);
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var cell = go.AddComponent(typeof(Cell)) as Cell;
        cell.x = i;
        cell.y = j;
        _gameObjects[i][j] = go;
    }

    IEnumerator UnloadScene()
    {
        Destroy(spot);
        var unloadTask = SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
        while(!unloadTask.isDone)
            yield return null;
        var scene = SceneManager.GetActiveScene();
        foreach(var obj in scene.GetRootGameObjects())
            obj.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        StateManager.controls.Player.Enable();
        yield return null;
    }

    void DropItem(InventoryItem item)
    {
        drops.Add(item);
        Inventory.instance.AddItem(item);
        if(drops.Count() == _mineItems.Length)
        {
            _unloading = true;
            CoroutineManager.LaunchCoroutine(UnloadScene());
        }
    }

    void HitCell(int x, int y, int amount)
    {
        if(amount == 0 || _board[x][y] < 0)
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

    private void SetFocus(int x, int y)
    {
        _focus = new Vector2Int(x, y);
        focusUI.position = GetPosition(x, y) + Vector3.back;
        _eventSystem.SetSelectedGameObject(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameState.instance.inputBlocked) return;
        var mousePos = Mouse.current.position.ReadValue();
        if(mousePos != _lastMousePos)
        {
            var ray = _camera.ScreenPointToRay(mousePos);
            var found = false;
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                var cell = hit.collider.transform.GetComponentInParent<Cell>();
                if(cell != null)
                {
                    SetFocus(cell.x, cell.y);
                    found = true;
                }
            }
            if(!found)
            {
                _focus = new Vector2Int(-1, 0);
                focusUI.position = new Vector3(-15, -15, -1);
            }
            _lastMousePos = mousePos;
        }
    }
}
