using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChessTile : MonoBehaviour
{
    public TileSlot tileSlot;
    public bool isSelected;
    public Vector2 pos;
    public Material material;
    public UnityEvent<ChessTile> OnMouseDown, OnMouseSelected = new UnityEvent<ChessTile>();

    private MeshRenderer _meshRenderer;
    private float _dissolveSpeed = 15f;
    private float _lerpingSpeed = 5f;
    private float _minLerpingDistance = .01f;
    private bool _isTurningOn, _deltaSelected;
    private Coroutine _movingInstance;

    public void Turn(bool on)
    {
        StartCoroutine(Dissolve(on));
    }

    public void Select(bool isClicked)
    {
        if (isClicked)
        {
            OnMouseDown.Invoke(this);
        }

        _deltaSelected = true;

        if (isSelected)
            return;

        isSelected = true;

        OnMouseSelected.Invoke(this);
    }

    public void DeSelect()
    {
        isSelected = false;
    }

    private IEnumerator LerpToPoint(Vector3 point)
    {
        float curSpeed = _lerpingSpeed * Time.fixedDeltaTime;

        while(Vector3.Distance(transform.position, point) > _minLerpingDistance)
        {
            transform.position = Vector3.Lerp(transform.position, point,
                curSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Dissolve(bool positive)
    {
        _isTurningOn = positive;
        float dissolveSpeed;

        if (positive)
        {
            dissolveSpeed = _dissolveSpeed;
        }
        else
        {
            dissolveSpeed = -_dissolveSpeed;
        }

        while (_isTurningOn == positive)
        {
            float addingDissolve =
                Mathf.Clamp
                (material.GetFloat("Dissolve") + 
                dissolveSpeed * Time.fixedDeltaTime
                , -1, 1);

            material.SetFloat("Dissolve", addingDissolve);

            yield return new WaitForFixedUpdate();
        }
    }

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        material = _meshRenderer.material;
        material.SetFloat("Dissolve", 0);

        ChessSystem.instance.tiles.Add(this);
        if (ChessSystem.instance.tiles.Count >= 64)
        {
            ChessSystem._onReadyToUse.Invoke();
        }
    }

    private void LateUpdate()
    {
        if (_deltaSelected)
        {
            _deltaSelected = false;
        }
        else
        {
            DeSelect();
        }
    }
}
public enum TileSlot {  Player, Enemy, Wall, PlayerStep, Open}
