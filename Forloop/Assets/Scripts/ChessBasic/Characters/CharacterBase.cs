using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBase : MonoBehaviour, IStepable
{
    public static CharacterBase instance;
    public Vector2 boardPos;
    public TileSlot characterType;

    [SerializeField] private float _minRotationLerpingStopDistance,
        _minMovingLerpingStopDistance,
        _rotationSpeed, _moveSpeed;
    protected Rigidbody _rigidbody;
    private float _spawnUpperPos = .5f;
    protected TurningDirection _currentDirection;
    private UnityEvent _OnRotationComplete = new UnityEvent();

    public virtual void OnYourStep()
    {
        // Some realization
    }

    public void MoveToPoint(Vector2 boardPos)
    {
        _OnRotationComplete.AddListener(() => StartCoroutine(MoveToPos(boardPos)));
        RotateToTile(boardPos);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        ChessSystem._onReadyToUse.AddListener(() => Setup());
    }

    private void Setup()
    {
        ChessTile tile = ChessSystem.ConvertCordinates(boardPos);
        tile.tileSlot = characterType;

        transform.position = new Vector3(
            tile.transform.position.x,
            tile.transform.position.y + _spawnUpperPos,
            tile.transform.position.z);
    }

    private void RotateToTile(Vector2 tilePos)
    {
        if (tilePos.x > boardPos.x)
        {
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, -90, 0),
                TurningDirection.Right));
        }
        else if (tilePos.x < boardPos.x)
        {
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 90, 0),
                TurningDirection.Left));
        }
        else if (tilePos.y > boardPos.y)
        {
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 180, 0), 
                TurningDirection.Up));
        }
        else if (tilePos.y < boardPos.y)
        {
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 0, 0),
                TurningDirection.Down));
        }
    }

    private IEnumerator MoveToPos(Vector2 boardPos)
    {
        Vector3 pos = ChessSystem.ConvertCordinates(boardPos).transform.position;
        pos.y = transform.position.y;

        while (Vector3.Distance(pos, transform.position) >
            _minMovingLerpingStopDistance)
        {
            _rigidbody.MovePosition(
                Vector3.Lerp(transform.position, pos, _moveSpeed * 
                Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }

        ChessSystem.ConvertCordinates(boardPos).tileSlot = characterType;
    }

    private IEnumerator RotateToRotation(Quaternion rotation,
        TurningDirection dir)
    {
        yield return new WaitForSeconds(.5f);

        _currentDirection = dir;

        while (Mathf.Abs(Quaternion.Angle(transform.rotation, rotation)) >
                _minRotationLerpingStopDistance && _currentDirection == dir)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                rotation, _rotationSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }

        if(_currentDirection == dir)
        {
            _currentDirection = TurningDirection.None;
        }

        _OnRotationComplete.Invoke();
        _OnRotationComplete.RemoveAllListeners();
    }

    public enum TurningDirection
    {
        Down, Up, Right, Left, None
    }
}
