using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float rotateSpeed = 5;
    public float distToPlayer = 10;
    public Transform focalPoint;

    private RaycastHit _closeZoom;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;

        transform.RotateAround(focalPoint.position, transform.parent.right, -vertical);
        transform.parent.Rotate(Vector3.up * horizontal);

        var ray = new Ray(focalPoint.position, -transform.forward);
        Debug.DrawRay(focalPoint.position, -transform.forward * distToPlayer, Color.magenta);
        //disttoplayer
        if (Physics.Raycast(ray, out _closeZoom, distToPlayer, ~LayerMask.NameToLayer("Terrain")))
        {
            transform.position = focalPoint.position - transform.forward * (_closeZoom.distance - 0.1f);
        }
        else
        {
            transform.position = focalPoint.position - transform.forward * (distToPlayer - 0.1f);
        }
    }
}
