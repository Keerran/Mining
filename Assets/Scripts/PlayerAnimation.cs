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
        _animator.SetFloat("Speed", _movement.magnitude);
    }
}
