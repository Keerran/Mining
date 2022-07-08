using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MineSpot : Interactable
{
    public override void Interact()
    {
        Debug.Log("interact");
        StartCoroutine(InteractCoroutine());
    }

    IEnumerator InteractCoroutine()
    {
        var loadTask = SceneManager.LoadSceneAsync("Mining", LoadSceneMode.Additive);
        foreach(var obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if(gameObject != obj && obj.tag != "GameController")
                obj.SetActive(false);
        }

        while(!loadTask.isDone)
            yield return null;

        var controller = GameObject.FindGameObjectWithTag("MinigameController");
        controller.GetComponent<Mining>().spot = gameObject;
        Cursor.lockState = CursorLockMode.None;
        StateManager.controls.Player.Disable();
        gameObject.SetActive(false);
    }

    public override void OnFocus()
    {
        Debug.Log("focus");
    }

    public override void OnUnfocus()
    {
        Debug.Log("unfocus");
    }
}
