using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float maxAngle;
    public float minAngle;
    public float rotateSpeed = 5;
    public float distToPlayer = 10;
    public Transform focalPoint;
    public LayerMask layerMask;

    private float _xRotation, _yRotation = 10 * Mathf.Deg2Rad;

    // Start is called before the first frame update
    void Start()
    {

    }
    

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed * 0.01f;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed * 0.01f;

        _yRotation += vertical;
        _yRotation = Mathf.Clamp(_yRotation, minAngle * Mathf.Deg2Rad, maxAngle * Mathf.Deg2Rad);
        _xRotation = (_xRotation - horizontal) % (2 * Mathf.PI);

        var pos = new Vector3(
            Mathf.Sin(_yRotation) * Mathf.Cos(_xRotation),
            Mathf.Cos(_yRotation),
            Mathf.Sin(_yRotation) * Mathf.Sin(_xRotation)
        );

        transform.position = pos + focalPoint.position;
        transform.LookAt(focalPoint);

        var ray = new Ray(focalPoint.position, -transform.forward);
        var distance = distToPlayer;
        if (Physics.Raycast(ray, out RaycastHit closeZoom, distToPlayer, layerMask))
        {
            distance = closeZoom.distance;
        }

        transform.position = focalPoint.position - transform.forward * (distance - 0.1f);
    }
}
