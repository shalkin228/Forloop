using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Thunder : BasicAttack
{
    public UnityEvent OnAttacked = new UnityEvent();

    [SerializeField] private GameObject _damageTrigger;
    [SerializeField] private float _stopDuration, _damagePhaseDuration, 
        _RemoveAfterAttackDuration;
    private bool _isFollowingPlayer;

    public static Thunder SpawnStandartThunder()
    {
        GameObject thunderObj = Resources.Load<GameObject>("Thunder");
        return Instantiate(thunderObj).GetComponent<Thunder>();
    }

    private void FixedUpdate()
    {
        if (_isFollowingPlayer)
        {
            var newPos = transform.position;
            newPos.x = Mathf.Lerp(newPos.x, PlayerBase.instance.transform.position.x,
                1);
            transform.position = newPos;
        }
    }

    protected override IEnumerator Start()
    {
        Vector2 newPos = PlayerBase.instance.transform.position;
        newPos.y += _spawnUpperPlayer;
        transform.position = newPos;

        _isFollowingPlayer = true;

        yield return StartCoroutine(base.Start());
    }

    protected override IEnumerator Attack()
    {
        _isFollowingPlayer = false;

        yield return new WaitForSeconds(_stopDuration);

        _damageTrigger.SetActive(true);

        yield return new WaitForSeconds(_RemoveAfterAttackDuration);

        OnAttacked.Invoke();
        Destroy(gameObject);
    }
}
