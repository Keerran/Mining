using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    public bool Running => _controls.Player.Run.IsPressed();

    public float speed = 4;
    public float runMagnitude = 3;
    public float jumpPower;
    public float magnitude { get; private set; }
    public float turnSmoothing;

    private Controls _controls;
    private Rigidbody _rigidbody;
    private Transform _camera;
    private Vector3 _moveDir;
    private Vector3 _combinedRaycast;
    private RaycastHit _groundHit;
    private float _gravity;
    private Vector3 _turnVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main.transform;
        _controls = StateManager.controls;
        _controls.Player.Jump.performed += ctx =>
        {
            if (IsGrounded())
                _gravity = jumpPower;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
            return;

        if (GameState.instance.inputBlocked)
        {
            magnitude = 0;
            _moveDir = Vector3.zero;
            return;
        }

        var move = _controls.Player.Move.ReadValue<Vector2>();
        var direction = _camera.forward * move.y + _camera.right * move.x;

        _moveDir = direction.ZeroY().normalized;
        magnitude = Mathf.Clamp01(Mathf.Abs(move.y) + Mathf.Abs(move.x));

        if (magnitude < 0.1)
            magnitude = 0;
        else if (Running)
            magnitude = runMagnitude;
    }


    void FixedUpdate()
    {
        if (!IsGrounded())
            _gravity += Physics.gravity.y * Time.fixedDeltaTime * 2f;

        var targetVelocity = _moveDir * (magnitude * speed);
        var velocity = Vector3.SmoothDamp(_rigidbody.velocity.ZeroY(), targetVelocity, ref _turnVelocity, turnSmoothing, 20f * (Running ? 2 : 1));
        _rigidbody.velocity = velocity + _gravity * Vector3.up;
        if (velocity.magnitude != 0)
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, velocity.normalized);
        var floorMovement = new Vector3(_rigidbody.position.x, FindFloor().y + 1f, _rigidbody.position.z);

        if (IsGrounded() && floorMovement != _rigidbody.position && _rigidbody.velocity.y <= 0)
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
