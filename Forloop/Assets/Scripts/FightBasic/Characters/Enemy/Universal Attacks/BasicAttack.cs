using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _spawnUpperPlayer;

    protected virtual IEnumerator Start()
    {
        yield return new WaitForSeconds(_attackCooldown);

        StartCoroutine(Attack());
    }

    protected virtual IEnumerator Attack()
    {
        yield break;
    }
}
