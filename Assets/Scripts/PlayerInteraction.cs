using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public Interactable focus = null;

    private Camera _camera;
    private LayerMask _layerMask;

    void Start()
    {
        _camera = Camera.main;
    }

    private bool test;

    void Update()
    {
        if(focus != null)
        {
            if(Input.GetButtonUp("Interact"))
            {
                focus.Interact();
            }
            if(!CanInteract(focus.transform))
            {
                RemoveFocus();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if(interactable != null && interactable != focus && CanInteract(other.transform))
        {
            SetFocus(interactable);
        }
    }

    void OnTriggerExit(Collider other)
    {
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
