using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject inventory;

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
            GameState.instance.paused = !GameState.instance.paused;

        if (Input.GetButtonDown("Inventory"))
        {
            var open = !inventory.activeSelf;
            GameState.instance.inputBlocked = open;
            inventory.SetActive(open);
            Cursor.lockState = open
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }
    }
}
