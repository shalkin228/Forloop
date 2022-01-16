using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessTile : MonoBehaviour
{
    public TileSlot tileSlot;
    public bool isSelected;
    public Vector2 pos;
    public Material material;

    private MeshRenderer _meshRenderer;
    private float _dissolveSpeed = 3f;
    private bool _isTurningOn, _deltaSelected;

    public void Turn(bool on)
    {
        StartCoroutine(Dissolve(on));
    }

    public void Select(bool isClicked)
    {
        _deltaSelected = true;

        if (isSelected)
            return;

        isSelected = true;
    }

    public void DeSelect()
    {
        isSelected = false;
        StartCoroutine(Dissolve(false));
    }

    private IEnumerator Dissolve(bool positive)
    {
        yield return new WaitForSeconds(.5f);

        _isTurningOn = positive;

        float dissolveSpeed = positive ? _dissolveSpeed : -_dissolveSpeed;

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
public enum TileSlot { Open, Player, Wall, Enemy, PlayerStep}
