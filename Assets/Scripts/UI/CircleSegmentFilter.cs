using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

public class CircleSegmentFilter : MonoBehaviour, ICanvasRaycastFilter
{
    private Image _image;
    private RectTransform _rect;

    void Start()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        var thetaMin = transform.eulerAngles.z;
        var thetaMax = thetaMin + _image.fillAmount * 360;
        var theta = 180 - Vector2.SignedAngle(Vector2.down, sp - _rect.position.XY());
        return Between(theta, thetaMin, thetaMax);
    }
}
