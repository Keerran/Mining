using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using UnityEngine;
using UnityEngine.Events;

public class GameState : ScriptableObject
{
    private static GameState _instance;
    public static GameState instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.FindObjectsOfTypeAll<GameState>().FirstOrDefault();

            #if UNITY_EDITOR
            if(!_instance)
                _instance = CreateInstance<GameState>();
            #endif

            return _instance;
        }
    }

    void Awake()
    {
        Load();
    }

    public UnityEvent<bool> onPauseChange = new UnityEvent<bool>();

    [SerializeField]
    private bool _paused;
    public bool paused
    {
        get => _paused;
        set
        {
            _paused = value;
            onPauseChange.Invoke(_paused);
        }
    }

    public void Save()
    {
        var data = new SaveData
        {
            itemIds = Inventory.instance.items.ToDictionary(stack => (stack.item.id, stack.size))
        };

        var bytes = MessagePackSerializer.Serialize(data);
        var path = Application.persistentDataPath + "/save.bin";
        File.WriteAllBytes(path, bytes);
        Debug.Log("Save complete.");
    }

    public void Load()
    {
        var path = Application.persistentDataPath + "/save.bin";
        if(File.Exists(path))
        {
            var bytes = File.ReadAllBytes(path);
            var data = MessagePackSerializer.Deserialize<SaveData>(bytes);

            Inventory.instance.LoadItems(data);

            Debug.Log("Load complete");
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Window/GameState")]
    public static void ShowState()
    {
        UnityEditor.Selection.activeObject = instance;
    }
    #endif
}
