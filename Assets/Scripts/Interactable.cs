using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent focusEvent;
    public UnityEvent interactEvent;

    void Awake()
    {
        if(focusEvent == null)
            focusEvent = new UnityEvent();

        if(interactEvent == null)
            interactEvent = new UnityEvent();
    }
}
