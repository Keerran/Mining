using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x, y;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Deconstruct(out int x, out int y)
    {
        x = this.x;
        y = this.y;
    }
}
