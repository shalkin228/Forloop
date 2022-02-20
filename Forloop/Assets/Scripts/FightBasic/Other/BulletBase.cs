using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BulletBase : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] private int _damage;
    [SerializeField] private UnityEvent _onCollided;
    [SerializeField] private GameObject _explosion;
    protected Rigidbody2D _rigidbody2D;

    protected virtual void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        _rigidbody2D.MovePosition(transform.position +
            transform.right * Time.fixedDeltaTime * _moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamagable>
            (out IDamagable damagble))
        {
            damagble.Damage(_damage);
        }

        _onCollided.Invoke();

        Instantiate(_explosion, transform.position, 
            Quaternion.identity);

        Destroy(gameObject);
    }
}
