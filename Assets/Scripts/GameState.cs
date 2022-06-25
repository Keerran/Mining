using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }

    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Window/GameState")]
    public static void ShowState()
    {
        UnityEditor.Selection.activeObject = instance;
    }
    #endif
}
