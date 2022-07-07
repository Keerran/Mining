using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Movement _movement;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponentInParent<Movement>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetFloat("MoveSpeed", _movement.magnitude * _movement.speed);
        // _animator.SetFloat("InputMagnitude", _movement.magnitude);
        if (_movement.magnitude > 0.1)
        {
            _animator.SetBool("IsRunning", _movement.Running);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }
    }
}
