using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    public abstract void OnFocus();

    public abstract void Interact();

    public abstract void OnUnfocus();
}
