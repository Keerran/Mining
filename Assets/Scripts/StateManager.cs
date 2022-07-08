using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject inventory;

    public static Controls controls { get; private set; }

    void Awake()
    {
        controls = new Controls();
        controls.UI.Pause.performed += ctx =>
            GameState.instance.paused = !GameState.instance.paused;
        controls.Player.Inventory.performed += ctx => {
            if(GameState.instance.paused)
                return;
            var open = !inventory.activeSelf;
            GameState.instance.inputBlocked = open;
            inventory.SetActive(open);
            Cursor.lockState = open
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        };
        Cursor.lockState = CursorLockMode.Locked;
        controls.Player.Cursor.performed += ctx => 
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
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
