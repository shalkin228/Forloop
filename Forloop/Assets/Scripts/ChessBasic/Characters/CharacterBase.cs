using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBase : MonoBehaviour, IComparer
{
    public static CharacterBase instance;
    public Vector2 boardPos;
    public TileSlot characterType;

    protected Rigidbody _rigidbody;
    protected float _spawnUpperPos = .5f;
    protected UnityEvent _OnRotationComplete = new UnityEvent();
    protected UnityEvent _OnMoveComplete = new UnityEvent();
    protected Coroutine _rotationInstance;

    private readonly float _minRotationLerpingStopDistance = 1f;
    private readonly float _minMovingLerpingStopDistance = 1e-05f;
    private readonly float _rotationSpeed = 14;
    private readonly float _moveSpeed = 15;


    public int Compare(object x, object y)
    {
        CharacterBase character = (CharacterBase)x;
        CharacterBase character2 = (CharacterBase)y;

        if ((int)character.characterType == (int)character2.characterType)
        {
            return 0;
        }

        return (int)character.characterType > (int)character2.characterType
            ? 1 : -1;
    }

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

    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        ChessSystem._onReadyToUse.AddListener(() => Setup());
    }

    protected void Setup()
    {
        ChessTile tile = ChessSystem.ConvertCordinates(boardPos);
        tile.tileSlot = characterType;

        transform.position = new Vector3(
            tile.transform.position.x,
            tile.transform.position.y + _spawnUpperPos,
            tile.transform.position.z);
    }

    protected void RotateToTile(Vector2 tilePos)
    {
        if (tilePos.x > boardPos.x)
        {
            if(_rotationInstance != null)
            {
                StopCoroutine(_rotationInstance);
            }

            _rotationInstance = 
                StartCoroutine(RotateToRotation(Quaternion.Euler(0, -90, 0)
                ));
        }
        else if (tilePos.x < boardPos.x)
        {
            if (_rotationInstance != null)
            {
                StopCoroutine(_rotationInstance);
            }

            _rotationInstance =
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 90, 0)
                ));
        }
        else if (tilePos.y > boardPos.y)
        {
            if (_rotationInstance != null)
            {
                StopCoroutine(_rotationInstance);
            }

            _rotationInstance =
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 180, 0) 
                ));
        }
        else if (tilePos.y < boardPos.y)
        {
            if (_rotationInstance != null)
            {
                StopCoroutine(_rotationInstance);
            }

            _rotationInstance =
            StartCoroutine(RotateToRotation(Quaternion.Euler(0, 0, 0)
                ));
        }
    }

    protected IEnumerator MoveToPos(Vector2 boardPos)
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
        ChessSystem.ConvertCordinates(this.boardPos).tileSlot = TileSlot.Open;

        this.boardPos = boardPos;

        _OnMoveComplete.Invoke();
        _OnMoveComplete.RemoveAllListeners();
    }

    protected IEnumerator RotateToRotation(Quaternion rotation)
    {
        while (Mathf.Abs(Quaternion.Angle(transform.rotation, rotation)) >
                _minRotationLerpingStopDistance)
        {
            _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,
                rotation, _rotationSpeed * Time.fixedDeltaTime));

            yield return new WaitForFixedUpdate();
        }

        _rotationInstance = null;

        _OnRotationComplete.Invoke();
        _OnRotationComplete.RemoveAllListeners();
    }
}
