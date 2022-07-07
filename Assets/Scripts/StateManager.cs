using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject inventory;

    private Controls _controls;

    void Awake()
    {
        _controls = new Controls();
        _controls.Player.Pause.performed += ctx =>
            GameState.instance.paused = !GameState.instance.paused;
        _controls.Player.Inventory.performed += ctx => {
            if(GameState.instance.paused)
                return;
            var open = !inventory.activeSelf;
            GameState.instance.inputBlocked = open;
            inventory.SetActive(open);
            Cursor.lockState = open
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        };
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState.instance.onPauseChange.AddListener(Pause);
    }

    void Pause(bool paused)
    {
        pauseMenu.SetActive(paused);
        Cursor.lockState = paused
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}
