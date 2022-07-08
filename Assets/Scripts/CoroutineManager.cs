using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;

    void Awake()
    {
        if(_instance == null)
            _instance = this;
        else
            Destroy(this);
    }

    public static void LaunchCoroutine(IEnumerator coroutine)
    {
        _instance.StartCoroutine(coroutine);
    }
}
