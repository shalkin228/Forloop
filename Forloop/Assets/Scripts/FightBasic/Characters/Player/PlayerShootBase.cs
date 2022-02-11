using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShootBase : PlayerBase
{
    [SerializeField] protected Transform _head, _bulletSpawn;
    [SerializeField] protected string _bulletName;
    [SerializeField] protected UnityEvent _onShoot;
    [SerializeField] protected float _shootCooldown;
    protected bool _canShoot = true;
    protected Camera _mainCamera;

    protected override void Start()
    {
        base.Start();

        _mainCamera = GameObject.FindGameObjectWithTag("LocationCamera")
            .GetComponent<Camera>();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        var mouse = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouse - _head.position;
        dir.z = 0;
        _head.right = dir;
    }

    protected virtual void Shoot()
    {
        if (!_canShoot) return;

        var bullet = Resources.Load(_bulletName);
        Instantiate(bullet, _bulletSpawn.position, 
            _bulletSpawn.rotation);

        StartCoroutine(ShootCooldown(_shootCooldown));

        _onShoot.Invoke();
    }

    protected virtual IEnumerator ShootCooldown(float duration)
    {
        _canShoot = false;

        yield return new WaitForSeconds(duration);

        _canShoot = true;
    }
}
