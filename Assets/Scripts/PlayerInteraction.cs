using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Interactable focus = null;

    private Camera _camera;
    private LayerMask _layerMask;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(transform.position + 1.8f * Vector3.up, _camera.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, 10))
        {
            var interactable = hit.collider.GetComponent<Interactable>();
            if(interactable)
            {
                SetFocus(interactable);
                return;
            }
        }
        RemoveFocus();
    }

    public void SetFocus(Interactable value)
    {
        focus = value;
    }

    public void RemoveFocus()
    {
        focus = null;
    }
}
