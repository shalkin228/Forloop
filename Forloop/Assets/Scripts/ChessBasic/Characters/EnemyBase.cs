using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : CharacterBase
{
    [SerializeField] protected int _instantiateOnStep;
    [SerializeField] protected GameObject _fightEnemy;
    [SerializeField] protected float _lightMaxIntensity, _lightTurnSpeed;
    [SerializeField] protected Vector2[] _path;
    protected int _currentPathNum;
    protected MeshRenderer _meshRenderer;
    protected BoxCollider _collider;
    protected List<Light> _eyes = new List<Light>();
    protected UnityEvent _onEyeTurnedRed = new UnityEvent();

    private UnityAction _setupAction;

    public void Remove()
    {
        ChessSystem.ConvertCordinates(boardPos).tileSlot =
            TileSlot.Open;
        ChessSystem.instance.characters.Remove(this);
        Destroy(gameObject);
    }

    public override void OnYourStep()
    {
        if (!_meshRenderer.enabled)
        {
            ChessSystem.NextHalfStep(0);
            return;
        }

        try
        {
            print(Time.time);
            MoveToPoint(_path[_currentPathNum]);
        }
        catch
        {
            TryStartFight();
        }
        _currentPathNum++;
        _OnMoveComplete.AddListener(() => StartFight());
    }

    protected override void Start()
    {
        base.Start();

        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;

        foreach(Transform child in transform)
        {
            if(child.TryGetComponent(out Light eyeLight))
            {
                _eyes.Add(eyeLight);
            }
        }
    }

    protected override void Setup()
    {
        _meshRenderer.enabled = false;

        if(_instantiateOnStep != ChessSystem.currentStep)
        {
            _setupAction = () => Setup();
            ChessSystem.onNextStep.AddListener(_setupAction);
        }
        else
        {
            try
            {
                ChessSystem.onNextStep.RemoveListener(_setupAction);
            }
            catch {}

            _collider.isTrigger = false;

            _meshRenderer.enabled = true;

            base.Setup();
        }
    }

    protected virtual IEnumerator EyeLightTurning(bool on)
    {
        float newIntensity = on ? _lightMaxIntensity : 0;

        while (Mathf.DeltaAngle(_eyes[0].intensity, newIntensity) > .01f)
        {
            yield return new WaitForFixedUpdate();

            float delta = Time.fixedDeltaTime * _lightTurnSpeed;

            foreach (Light eye in _eyes)
            {
                eye.intensity = Mathf.Lerp(eye.intensity, newIntensity, delta);
            }
        }

        _onEyeTurnedRed.Invoke();
        _onEyeTurnedRed.RemoveAllListeners();
    }

    protected virtual void TryStartFight()
    {
        bool playerFinded = false;

        foreach (ChessTile tile in ChessSystem.OverlapBox(boardPos, false))
        {
            if (tile.tileSlot == TileSlot.Player)
            {
                playerFinded = true;

                _OnRotationComplete.AddListener(() =>
                FightManager.StartFight
                (ChessSystem.instance.location,
                PlayerCharacterBase._currentPlayer._playerFightCharacter,
                _fightEnemy,
                this));
                RotateToTile(tile.pos);

                break;
            }
        }

        if (!playerFinded)
        {
            ChessSystem.NextHalfStep(0);
        }
    }

    protected virtual void StartFight()
    {
        bool playerFinded = false;

        foreach (ChessTile tile in ChessSystem.OverlapBox(boardPos, false))
        {
            if (tile.tileSlot == TileSlot.Player)
            {
                playerFinded = true;

                _OnRotationComplete.AddListener(() =>
                FightManager.StartFight
                (ChessSystem.instance.location,
                PlayerCharacterBase._currentPlayer._playerFightCharacter,
                _fightEnemy,
                this));
                RotateToTile(tile.pos);

                break;
            }
        }

        if (!playerFinded)
        {
            ChessSystem.NextHalfStep();
        }
    }
}
