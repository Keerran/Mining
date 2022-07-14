using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : NetworkBehaviour
{
    public Interactable focus = null;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
        StateManager.controls.Player.Interact.performed += ctx => {
            if(focus != null && !GameState.instance.paused)
                focus.Interact();
        };
    }

    void Update()
    {
        if(!base.IsOwner)
            return;
        if(focus != null)
        {
            if(!CanInteract(focus.transform))
            {
                RemoveFocus();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(!base.IsOwner)
            return;
        var interactable = other.GetComponent<Interactable>();
        if(interactable != null && interactable != focus && CanInteract(other.transform))
        {
            SetFocus(interactable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(!base.IsOwner)
            return;
        if(other.gameObject == focus?.gameObject)
        {
            RemoveFocus();
        }
    }

    private bool CanInteract(Transform obj)
    {
        var direction = obj.position - transform.position;
        return Vector3.Dot(direction, transform.forward) > 0;
    }

    public void SetFocus(Interactable value)
    {
        focus = value;
        value.OnFocus();
    }

    public void RemoveFocus()
    {
        focus?.OnUnfocus();
        focus = null;
    }
}
