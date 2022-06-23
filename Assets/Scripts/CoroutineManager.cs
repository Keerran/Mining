using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager instance { get; private set; }

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void LaunchCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
