using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterBase : CharacterBase
{
    protected ChessTile[] _currentTiles;

    public override void OnYourStep()
    {
        _currentTiles = ChessSystem.OverlapBox(boardPos, true);

        foreach(ChessTile tile in _currentTiles)
        {
            tile.OnMouseSelected.AddListener(OnMouseTileSelect);
            tile.OnMouseDown.AddListener(OnMouseTileDown);

            tile.Turn(true);
        }
    }

    private void OnMouseTileSelect(ChessTile tile)
    {
        RotateToTile(tile.pos);
    }

    private void OnMouseTileDown(ChessTile tile)
    {
        MoveToPoint(tile.pos);

        foreach (ChessTile stile in _currentTiles)
        {
            stile.Turn(false);

            stile.OnMouseSelected.RemoveAllListeners();
            stile.OnMouseDown.RemoveAllListeners();
        }
        _currentTiles = Array.Empty<ChessTile>();

        _OnMoveComplete.AddListener(() => ChessSystem.NextHalfStep());
    }
}
