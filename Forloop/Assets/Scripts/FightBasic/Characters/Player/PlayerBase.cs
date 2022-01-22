using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] protected float _maxFallSpeed, _maxMoveSpeed, _jumpForce;
    [SerializeField] protected InputAction _action, _jump;
    protected Rigidbody2D _rigidbody2D;
    protected float _horizontal;
    protected GroundCheck _groundCheck;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void OnEnable()
    {
        _action.Enable();
        _jump.Enable();

        _jump.performed += ctx =>
        {
            if (_groundCheck.isGrounded)
            {
                _rigidbody2D.velocity = new Vector2(0, _jumpForce);
            }
        };

        _action.performed += ctx => Move(ctx.ReadValue<float>());
        _action.canceled += ctx => _horizontal = 0;
    }

    private void Move(float horizontal)
    {
        _horizontal = horizontal;
    }

    private void OnDisable()
    {
        _jump.Disable();
        _action.Disable();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(_horizontal) > .5f)
        {
            _rigidbody2D.MovePosition((Vector2)transform.position +
                new Vector2(
                    (_horizontal > 0 ? _maxMoveSpeed : -_maxMoveSpeed)
                    * Time.fixedDeltaTime, 0));
        }

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
            Mathf.Clamp(_rigidbody2D.velocity.y, -_maxFallSpeed, _jumpForce));
    }
}
