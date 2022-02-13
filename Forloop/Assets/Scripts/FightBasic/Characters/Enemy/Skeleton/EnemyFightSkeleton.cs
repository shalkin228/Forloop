using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFightSkeleton : EnemyFightBase
{
    [SerializeField] private float _afterAttackCooldown, _firstAttackCooldown;
    [SerializeField] private Transform[] _stoneSockets;
    [SerializeField] private GameObject _stone;
    private bool _canAttack;
    private Attack _oldAttack;
    private int _reapitingAttack;

    protected override IEnumerator LerpToSpawnpoint()
    {
        _canAttack = false;

        yield return StartCoroutine(base.LerpToSpawnpoint());

        StartCoroutine(AttackCooldown(_firstAttackCooldown));
    }

    private void FixedUpdate()
    {
        if (_canAttack)
        {
            AttackPlayer();

            _canAttack = false;
        }
    }

    private void AttackPlayer()
    {
        Attack attack = GetRandomAttack();

        if (attack == _oldAttack && _reapitingAttack >= 2)
        {
            var deltaAttack = attack;

            while (true)
            {
                attack = GetRandomAttack();

                if (attack != deltaAttack)
                {
                    break;
                }
            }
        }
        else if (attack == _oldAttack)
        {
            _reapitingAttack++;
        }

        if(attack == Attack.StoneAttack)
        {
            StartCoroutine(SpawnStones());
        }
        else
        {
            Thunder.SpawnStandartThunder().OnAttacked.AddListener(
                () => StartCoroutine(AttackCooldown(_afterAttackCooldown)));        
        }

        _oldAttack = attack;
    }

    private IEnumerator AttackCooldown(float Cooldown)
    {
        yield return new WaitForSeconds(Cooldown);

        _canAttack = true;
    }

    private Attack GetRandomAttack()
    {
        return (Attack)Random.Range(0, 2);
    }

    private IEnumerator SpawnStones()
    {
        List<SkeletonStone> stones = new List<SkeletonStone>();

        foreach(Transform stoneSocket in _stoneSockets)
        {
            stones.Add(Instantiate(_stone, 
            stoneSocket.position, Quaternion.identity,
            transform.parent)
            .GetComponent<SkeletonStone>());

            stones[stones.Count - 1].target =
                PlayerBase.instance.transform.position;
        }

        while (!stones[stones.Count - 1].isReady)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return stones[stones.Count - 1].StartCoroutine
            (stones[stones.Count - 1].RotateToTarget
            (PlayerBase.instance.transform.position));

        yield return stones[stones.Count - 2].StartCoroutine
           (stones[stones.Count - 2].RotateToTarget
           (PlayerBase.instance.transform.position));

        stones[0].onAttacked.AddListener(() => StartCoroutine(AttackCooldown
            (_afterAttackCooldown)));
    }


    private enum Attack
    {
        StoneAttack, Thunder
    }
}
