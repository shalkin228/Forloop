using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(ActorBody))]
public class PlayerBase : MonoBehaviour, IDamagable
{
    public static PlayerBase instance;

    public bool canBeDamaged = true;

    [SerializeField] protected float _maxMoveSpeed, _maxHealth;
    [SerializeField] protected InputActionReference _action;
    protected ActorBody _actorBody;
    protected bool _canBeDamaged = true;
    protected float _horizontal, _jumpTimer, _fallTimer, _currentHealth;
    protected float _afterDamageCooldown = 1f;
    protected Vector2 _velocity;

    public void Damage(int damage)
    {
        if (!_canBeDamaged || !canBeDamaged) return;

        _currentHealth -= damage;

        if(_currentHealth <= 0)
        {
            Death();
        }

        StartCoroutine(AfterDamageCooldown());
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
    }

    protected IEnumerator AfterDamageAnimation()
    {
        bool alpha = true;

        var sprites = GetComponentsInChildren<SpriteRenderer>();

        while (!_canBeDamaged)
        {
            if (alpha)
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    var newColor = sprite.color;
                    newColor.a = .5f;

                    sprite.color = newColor;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in sprites)
                {
                    var newColor = sprite.color;
                    newColor.a = 1;

                    sprite.color = newColor;
                }
            }

            alpha = !alpha;

            yield return new WaitForSeconds(.2f);
        }

        foreach (SpriteRenderer sprite in sprites)
        {
            var newColor = sprite.color;
            newColor.a = 1;

            sprite.color = newColor;
        }
    }

    protected IEnumerator AfterDamageCooldown()
    {
        _canBeDamaged = false;

        StartCoroutine(AfterDamageAnimation());

        yield return new WaitForSeconds(_afterDamageCooldown);

        _canBeDamaged = true;
    }

    protected void Death()
    {
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        instance = this;

        _currentHealth = _maxHealth;
    }

    protected virtual void Start()
    {
        _actorBody = GetComponent<ActorBody>();
    }

    protected virtual void OnEnable()
    {
        _action.action.Enable();

        _action.action.performed += ctx => Move(ctx.ReadValue<float>());
        _action.action.canceled += ctx => _horizontal = 0;
    }

    protected virtual void Move(float horizontal)
    {
        _horizontal = horizontal;
    }

    protected virtual void OnDisable()
    {
        _action.action.Disable();
    }

    protected virtual void FixedUpdate()
    {
        if (Mathf.Abs(_horizontal) > .5f)
        {
            _velocity += Vector2.right *
                ((_horizontal > 0 ? _maxMoveSpeed : -_maxMoveSpeed) 
                * Time.fixedDeltaTime);
        }

        _actorBody.Move(_velocity);
        _velocity = Vector2.zero;
    }
}
