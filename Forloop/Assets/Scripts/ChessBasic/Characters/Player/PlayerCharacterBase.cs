using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterBase : CharacterBase
{
    protected ChessTile[] _currentTiles;

    public override void OnYourStep()
    {
        ChessTile[] tiles = ChessSystem.OverlapBox(boardPos, true);
    }
}
