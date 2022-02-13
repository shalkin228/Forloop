using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(ActorBody))]
public class EnemyFightBase : MonoBehaviour, IDamagable
{
    public static EnemyFightBase currentEnemy;
    public int currentHealth;

    protected ActorBody _actorBody;
    protected Vector2 _spawnPoint;
    protected bool _isKilled;

    public virtual void Damage(int damage)
    {
        if (_isKilled) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            _isKilled = true;
            StartCoroutine(Death());
        }
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;
    }

    public virtual IEnumerator Death()
    {
        foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = Color.red;
        }

        PlayerBase.instance.canBeDamaged = false;

        yield return new WaitForSeconds(1f);

        FightManager.StopFight();
    }

    protected virtual void Start()
    {
        _actorBody = GetComponent<ActorBody>();

        currentEnemy = this;

        _spawnPoint = 
            GameObject.FindGameObjectWithTag
            ("SpawnPoint").transform.position;

        StartCoroutine(LerpToSpawnpoint());
    }

    protected virtual IEnumerator LerpToSpawnpoint()
    {
        var newPos = _spawnPoint;
        newPos.y += 10;
        transform.position = newPos;

         while(Vector2.Distance
             (transform.position, _spawnPoint) > .01f)
         {
            Vector2 dif = Vector2.Lerp
                 (transform.position, _spawnPoint,
                 Time.fixedDeltaTime *2)
                 - (Vector2)transform.position;

             _actorBody.Move(dif);

             yield return new WaitForFixedUpdate();
         }
    }
}
