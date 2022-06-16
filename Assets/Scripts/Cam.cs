using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float rotateSpeed = 5;
    public float distToPlayer = 6;

    private Vector3 _offset;
    private RaycastHit closeZoom;

    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;

        transform.RotateAround(transform.parent.position, transform.parent.right, -vertical);
        transform.parent.Rotate(Vector3.up * horizontal);

        var ray = new Ray(transform.parent.position, -transform.forward);
        Debug.DrawRay(transform.parent.position, -transform.forward * distToPlayer, Color.magenta);
        //disttoplayer
        if (Physics.Raycast(ray, out closeZoom, distToPlayer, ~LayerMask.NameToLayer("Terrain")))
        {
            transform.position = transform.parent.position - transform.forward * (closeZoom.distance - 0.1f);
        }
        else
        {
            transform.position = transform.parent.position - transform.forward * (distToPlayer - 0.1f);
        }
    }
}
