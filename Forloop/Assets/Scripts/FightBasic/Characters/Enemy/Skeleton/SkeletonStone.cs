using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class SkeletonStone : BulletBase
{
    public UnityEvent onAttacked = new UnityEvent();
    public bool isAttacking;
    public bool isReady;
    public Vector3 target;

    public IEnumerator RotateToTarget(Vector3 target)
    {
        var player = PlayerBase.instance.transform.position;
        Vector3 dir = player - transform.position;
        dir.z = 0;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
            - 90f;
        while (Mathf.DeltaAngle(_rigidbody2D.rotation, angle)
            > .1f)
        {
            transform.rotation = Quaternion.Lerp
                (transform.rotation,
                Quaternion.Euler(0, 0, angle),
                Time.fixedDeltaTime * 4);

            yield return new WaitForFixedUpdate();
        }

        isAttacking = true;

        yield break;
    }

    private void OnDestroy()
    {
        onAttacked.Invoke();
    }

    protected override void Start()
    {
        base.Start();

        var newPos = transform.position;
        newPos.y += 6;
        transform.position = newPos;

        StartCoroutine(LerpToNormal());
    }

    private IEnumerator LerpToNormal()
    {
        var normalPos = transform.position;
        normalPos.y -= 6;

        while (Vector2.Distance
    (transform.position, normalPos) > .01f)
        {
            _rigidbody2D.MovePosition(Vector2.Lerp
                 (transform.position, normalPos,
                 Time.fixedDeltaTime * 4));

            yield return new WaitForFixedUpdate();
        }

        isReady = true;
    }

    protected override void Update()
    {
        if (!isAttacking) return;

        _rigidbody2D.MovePosition(transform.position +
             transform.up * Time.deltaTime * _moveSpeed);
    }
}
