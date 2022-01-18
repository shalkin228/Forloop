using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamSwitcher : MonoBehaviour
{
    [SerializeField] private InputAction _moveCam;
    private Animator _animator;


    private void OnEnable()
    {
        _moveCam.Enable();
        _moveCam.performed += ctx => MoveCam(ctx.ReadValue<Vector2>());

        _animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        _moveCam.Disable();
    }

    private void MoveCam(Vector2 direction)
    {
        if(direction.y > .5f)
        {
            _animator.Play("BoardView");
        }
        else if (direction.y < -.5f)
        {
            _animator.Play("Overview");
        }
    }
}
