using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    public Type type;

    private LevelBuilder _controller;
    private Collider[] _colliders;

    void Start()
    {
        _controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelBuilder>();
        if(_controller == null)
        {
            Debug.Log("No level builder found. Will not generate mining spots.");
            Destroy(this);
        }
        _colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (_controller.spots < _controller.maxSpots)
        {
            if(RandomOnMesh(out RaycastHit hit))
            {
                _controller.spots++;
                var rot = Quaternion.FromToRotation(Vector3.right, hit.normal);
                Instantiate(_controller.mineSpot, hit.point, rot);
            }
        }
    }

    Vector3[] Directions()
    {
        switch (type)
        {
            case Type.CORNER:
                return new[] {
                    Vector3.forward,
                    Vector3.right
                };
            case Type.DEADEND:
                return new[] { Vector3.forward };
            case Type.INTERSECTION:
                break;
            case Type.STRAIGHT:
                return new[] { -Vector3.forward, Vector3.forward };
            case Type.TJUNC:
                return new[] { -Vector3.forward };
        }
        return new Vector3[] { };
    }

    bool RandomOnMesh(out RaycastHit hit)
    {
        var size = 12f;
        var direction = transform.TransformDirection(Directions().Random());
        if(direction == Vector3.zero)
        {
            hit = new RaycastHit();
            return false;
        }
        var length = Random.Range(0f, size / 2);
        var side = Random.Range(0, 2) * 2 - 1;

        var ray = new Ray(
            transform.position + direction * length + Vector3.up * 1.5f,
            Vector3.Cross(direction, Vector3.up) * side
        );

        // Debug.DrawRay(
        //     ray.origin + Vector3.up,
        //     ray.direction * size / 2,
        //     Color.cyan,
        //     10000
        // );

        if(_colliders.Raycast(ray, out hit, size / 2))
        {
            return true;
        }

        hit = new RaycastHit();
        return false;
    }

    public enum Type
    {
        STRAIGHT,
        TJUNC,
        DEADEND,
        INTERSECTION,
        CORNER
    }
}
