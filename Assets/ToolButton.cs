using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    public MiningTool tool;

    private Mining _mining;
    private Image _image;

    void Start()
    {
        _mining = FindObjectOfType<Mining>();
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if(_mining.tool == tool)
            _image.color = new Color(1, 0, 0);
        else
            _image.color = new Color(1, 1, 1);
    }
}
