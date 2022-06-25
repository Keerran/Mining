using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool Running { get; private set; }

    public float speed = 7;
    public float jumpPower;

    private Rigidbody _rigidbody;
    private Transform _camera;
    private Vector3 _moveDir;
    private float _magnitude;
    private Vector3 _combinedRaycast;
    private RaycastHit _groundHit;
    private float _gravity;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Running = Input.GetButton("Run");
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        var direction = _camera.forward * vertical + _camera.right * horizontal;

        _moveDir = direction.ZeroY().normalized;
        _magnitude = Mathf.Clamp01(Mathf.Abs(vertical) + Mathf.Abs(horizontal));

        if (_magnitude < 0.1)
            _magnitude = 0;
        else if (Running)
            _magnitude *= 2;

        if(Input.GetButtonDown("Jump") && IsGrounded())
            _gravity = jumpPower;
    }


    void FixedUpdate()
    {
        if(!IsGrounded())
            _gravity += Physics.gravity.y * Time.fixedDeltaTime * 2f;

        _rigidbody.velocity = _moveDir * (_magnitude * speed) + _gravity * Vector3.up;
        if(_moveDir.magnitude != 0)
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, _moveDir);
        var floorMovement = new Vector3(_rigidbody.position.x, FindFloor().y + 1f, _rigidbody.position.z);

        if(IsGrounded() && floorMovement != _rigidbody.position && _rigidbody.velocity.y <= 0)
        {
            _rigidbody.MovePosition(floorMovement);
            _gravity = 0;
            _rigidbody.velocity = _rigidbody.velocity.ZeroY();
        }
    }

    private Vector3 RaycastFloor(float x, float z, float length)
    {
        return RaycastFloor(x, z, length, out _);
    }


    private Vector3 RaycastFloor(float x, float z, float length, out RaycastHit hit)
    {
        var point = transform.TransformPoint(x, -0.4f, z);

        Debug.DrawRay(point, Vector3.down, Color.magenta);

        return (Physics.Raycast(point, -Vector3.up, out hit, length)
            ? hit.point
            : Vector3.zero);
    }

    private int GetFloorAverage(float x, float z)
    {
        var hit = RaycastFloor(x, z, 1.1f);
        _combinedRaycast += hit;
        return hit != Vector3.zero ? 1 : 0;
    }

    private Vector3 FindFloor()
    {
        const float width = 0.4f;
        int average = 1;

        _combinedRaycast = RaycastFloor(0, 0, 1.1f);
        average += GetFloorAverage(width, 0)
            + GetFloorAverage(-width, 0)
            + GetFloorAverage(0, width)
            + GetFloorAverage(0, -width);

        return _combinedRaycast / average;
    }


    public bool IsGrounded()
    {
        return RaycastFloor(0, 0, .7f, out _groundHit) != Vector3.zero;
    }
}
