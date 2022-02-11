using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private int _damage;

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamagable>
            (out IDamagable damagble))
        {
            damagble.Damage(_damage);
        }
        else
        {
            return;
        }
    }
}
