using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBase : CharacterBase
{
    [SerializeField] protected int _instantiateOnStep;
    [SerializeField] protected float _lightMaxIntensity, _lightTurnSpeed;
    [SerializeField] protected Vector2[] _path;
    protected int _currentPathNum;
    protected MeshRenderer _meshRenderer;
    protected BoxCollider _collider;
    protected List<Light> _eyes = new List<Light>();

    private UnityAction _setupAction;

    public override void OnYourStep()
    {
        if (!_meshRenderer.enabled)
        {
            ChessSystem.NextHalfStep(0);
            return;
        }

        try
        {
            MoveToPoint(_path[_currentPathNum]);
        }
        catch
        {
            ChessSystem.NextHalfStep(0);
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
    }

    protected virtual void StartFight()
    {
        bool playerFinded = false;

        foreach (ChessTile tile in ChessSystem.OverlapBox(boardPos, false))
        {
            if (tile.tileSlot == TileSlot.Player)
            {
                playerFinded = true;

                RotateToTile(tile.pos);
                _OnRotationComplete.AddListener(() => 
                StartCoroutine(EyeLightTurning(true)));
            }
        }

        if (!playerFinded)
        {
            ChessSystem.NextHalfStep();
        }
    }
}
