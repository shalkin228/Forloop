using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MovingObject : MonoBehaviour
{
    [SerializeField] private float _movingSpeed;
    private float _startPos, _length;

    private void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
    }
}
