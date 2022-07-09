using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float maxAngle;
    public float minAngle;
    public float distToPlayer = 10;
    public LayerMask layerMask;
    public float smoothLookTime;
    public float smoothDistanceTime;

    private Controls _controls;
    private Transform _focalPoint;
    private Vector3 _currentLook;
    private Vector3 _lookVelocity;
    private float _distVelocity;
    private Rigidbody _playerRb;

    // Start is called before the first frame update
    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        _focalPoint = player.transform.Find("Cam Focal Point");
        _currentLook = _focalPoint.position;
        _playerRb = player.GetComponent<Rigidbody>();
        _controls = StateManager.controls;
    }


    // Update is called once per frame
    void Update()
    {
        var input = _controls.Player.Look.ReadValue<Vector2>() * 0.0025f;

        if (GameState.instance.inputBlocked)
            input = Vector2.zero;

        // if (input != Vector2.zero)
        // {
        //     transform.position += _playerRb.velocity * Time.deltaTime;
        // }
        _currentLook = Vector3.SmoothDamp(_currentLook, _focalPoint.position, ref _lookVelocity, smoothLookTime);

        // Camera position relative to current look in spherical coordinates
        var pos = transform.position - _currentLook;
        var r = pos.magnitude;
        var theta = Mathf.Acos(pos.y / r);
        var phi = Mathf.Atan2(pos.z, pos.x);

        theta = Mathf.Clamp(theta + input.y, minAngle * Mathf.Deg2Rad, maxAngle * Mathf.Deg2Rad);
        phi -= input.x;

        var distance = Mathf.SmoothDamp(r, distToPlayer, ref _distVelocity, smoothDistanceTime);
        // var distance = distToPlayer;

        pos = distance * new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi)
        );

        var targetPos = pos + _currentLook;
        
        // Move in front terrain between player and camera
        var ray = new Ray(_currentLook, pos);
        if (Physics.Raycast(ray, out var hit, pos.magnitude, layerMask))
            targetPos = hit.point;

        test.position = _currentLook;
        transform.position = targetPos;
        transform.LookAt(_currentLook);
    }

    public Transform test;
}
