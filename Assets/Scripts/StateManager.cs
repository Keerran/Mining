using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public GameObject pauseMenuPrefab;

    private GameObject _pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        GameState.instance.onPauseChange.AddListener(Pause);
    }

    void Pause(bool paused)
    {
        if(paused)
        {
            _pauseMenu = Instantiate(pauseMenuPrefab, Vector3.zero, Quaternion.identity);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Destroy(_pauseMenu);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
            GameState.instance.paused = !GameState.instance.paused;
    }
}
