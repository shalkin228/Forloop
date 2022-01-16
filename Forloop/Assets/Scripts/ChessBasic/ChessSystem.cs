using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class ChessSystem : MonoBehaviour
{
    public static int currentStep;
    public static ChessSystem instance;
    public static UnityEvent _onReadyToUse = new UnityEvent();
    public List<ChessTile> tiles = new List<ChessTile>();
    public List<IStepable> characters = new List<IStepable>();

    [SerializeField] private float _stepCooldown;
    private Camera _mainCamera;
    private int _currentHalfStep;

    public static void NextHalfStep()
    {

    } 

    public static ChessTile ConvertCordinates(Vector2 cordinates)
    {
        foreach(ChessTile tile in instance.tiles.ToArray())
        {
            if(tile.pos == cordinates)
            {
                return tile;
            }
        }
        return null;
    }

    public static ChessTile[] OverlapBox(Vector2 curPos, bool returnOnlyOpenTiles)
    {
        Vector2 cordinate = new Vector2();
        List<ChessTile> tiles = new List<ChessTile>();

        for (int i = 0; i < 4; i++)
        {
            cordinate = curPos;

            if(i == 0)
            {
                cordinate.x--;
                try
                {
                    if (ConvertCordinates(cordinate) != null ||
                        ConvertCordinates(cordinate).tileSlot != TileSlot.Open
                        && returnOnlyOpenTiles)
                    {
                        tiles.Add(ConvertCordinates(cordinate));
                    }
                }
                catch { }
            }
            else if(i == 1)
            {
                cordinate.x++;
                try
                {
                    if (ConvertCordinates(cordinate) != null ||
                        ConvertCordinates(cordinate).tileSlot != TileSlot.Open
                        && returnOnlyOpenTiles)
                    {
                        tiles.Add(ConvertCordinates(cordinate));
                    }
                }
                catch{}
            }
            else if (i == 2)
            {
                cordinate.y++;
                try
                {
                    if (ConvertCordinates(cordinate) != null ||
                        ConvertCordinates(cordinate).tileSlot != TileSlot.Open
                        && returnOnlyOpenTiles)
                    {
                        tiles.Add(ConvertCordinates(cordinate));
                    }
                }
                catch { }
            }
            else if (i == 3)
            {
                cordinate.y--;
                try
                {
                    if (ConvertCordinates(cordinate) != null ||
                        ConvertCordinates(cordinate).tileSlot != TileSlot.Open
                        && returnOnlyOpenTiles)
                    {
                        tiles.Add(ConvertCordinates(cordinate));
                    }
                }
                catch { }
            }
        }
        return tiles.ToArray();
    }

    private IEnumerator NextHalfStepCooldown()
    {
        yield return new WaitForSeconds(.5f);

        instance._currentHalfStep++;

        instance.characters[instance._currentHalfStep].OnYourStep();
    }

    private void Awake()
    {
        instance = this;

        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        Ray mouseRay = _mainCamera.ScreenPointToRay(mousePos);

        RaycastHit[] hits = Physics.RaycastAll(mouseRay, 100);

        foreach (RaycastHit hit in hits)
        {
            if(hit.collider.TryGetComponent(out ChessTile tile))
            {
                hit.collider.GetComponent<ChessTile>().Select(
                    Input.GetMouseButtonDown(0));
                break;
            }
        }
    }
}
