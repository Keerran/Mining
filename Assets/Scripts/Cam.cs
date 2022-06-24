using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float maxAngle;
    public float minAngle;
    public float rotateSpeed = 5;
    public float distToPlayer = 10;
    public LayerMask layerMask;

    private Transform _focalPoint;
    private Vector3 _currentLook;
    private Vector3 _lookVelocity;
    private Vector3 _moveVelocity;
    private float _distVelocity;
    private Rigidbody _playerRb;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _focalPoint = player.transform.Find("Cam Focal Point");
        _currentLook = _focalPoint.position;
        _playerRb = player.GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed * 0.01f;
        float vertical = Input.GetAxis("Mouse Y") * rotateSpeed * 0.01f;
        
        if(horizontal == 0 && vertical == 0)
        {
            transform.position -= _playerRb.velocity * Time.deltaTime;
        }
        _currentLook = Vector3.SmoothDamp(_currentLook, _focalPoint.position, ref _lookVelocity, 0.3f);
        var pos = transform.position - _currentLook;
        var r = pos.magnitude;
        var theta = Mathf.Acos(pos.y / r);
        var phi = Mathf.Atan2(pos.z, pos.x);

        theta = Mathf.Clamp(theta + vertical, minAngle * Mathf.Deg2Rad, maxAngle * Mathf.Deg2Rad);
        phi -= horizontal;

        var distance = Mathf.SmoothDamp(r, distToPlayer, ref _distVelocity, 0.3f);

        pos = distance * new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi)
        );

        transform.position = pos + _currentLook;
        transform.LookAt(_currentLook);
    }
}
