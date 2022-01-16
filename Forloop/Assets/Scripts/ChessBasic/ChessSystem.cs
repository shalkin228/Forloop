using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class ChessSystem : MonoBehaviour
{
    public static int currentStep
    {
        get
        {
            return instance._currentStep;
        }
        set
        {
            instance._currentHalfStep = 0;
            instance._currentStep = value;

            if(value == 1)
            {
                instance.StartChessSteps();
            }
            else
            {
                NextHalfStep(1);
            }
        }
    }
    public static ChessSystem instance;
    public static UnityEvent _onReadyToUse = new UnityEvent();
    public List<ChessTile> tiles = new List<ChessTile>();
    public List<CharacterBase> characters = new List<CharacterBase>();

    [SerializeField] private float _stepCooldown;
    private Camera _mainCamera;
    private int _currentHalfStep, _currentStep;

    public static void NextHalfStep(float duration = .5f)
    {
        instance.StartCoroutine(instance.NextHalfStepCooldown(duration));
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
                        ConvertCordinates(cordinate).tileSlot == TileSlot.Open
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
                        ConvertCordinates(cordinate).tileSlot == TileSlot.Open
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
                        ConvertCordinates(cordinate).tileSlot == TileSlot.Open
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
                        ConvertCordinates(cordinate).tileSlot == TileSlot.Open
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

    public void StartChessSteps()
    {
        StartCoroutine(NextFirstStepCooldown(1f));
    }

    private IEnumerator NextFirstStepCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        ResortCharacters();

        instance.characters[instance._currentHalfStep].OnYourStep();
        instance._currentHalfStep++;

       /* if (_currentHalfStep >= instance.characters.Count)
        {
            currentStep++;
        }*/
    }

    private IEnumerator NextHalfStepCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        instance.characters[0].OnYourStep();
        instance._currentHalfStep++;

        /*if (_currentHalfStep >= instance.characters.Count)
        {
            currentStep++;
        }*/
    }

    private void ResortCharacters()
    {
        foreach(CharacterBase character in FindObjectsOfType<CharacterBase>())
        {
            this.characters.Add(character);
        }

        CharacterBase[] characters = this.characters.ToArray();

        Array.Sort(characters);

        this.characters = new List<CharacterBase>(characters);
    }

    private void Awake()
    {
        instance = this;

        _mainCamera = Camera.main;

        currentStep++;
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
