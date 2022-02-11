using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderDamage : DamageTrigger
{
    [SerializeField] private ParticleSystem _partickleSystem;
    private bool _canAttack = false;

    public void OnAnimationComplete() 
    {
        _canAttack = true;

        _partickleSystem.Play();
        _partickleSystem.gameObject.transform.SetParent(null);
    }
    

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if(!_canAttack)  return;

        base.OnTriggerStay2D(collision);

        _canAttack = false;
    }
}
