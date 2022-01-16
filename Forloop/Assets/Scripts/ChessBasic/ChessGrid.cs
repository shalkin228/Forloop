using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGrid : MonoBehaviour
{
    [SerializeField] private float _XOffset, _YOffset;

    private void Awake()
    {
        SpawnTiles(7, 7, _XOffset, _YOffset);
    }

    private void SpawnTiles(float tileX, float tileY, float xOffset, float yOffset)
    {
        var firstTileTransform = transform.GetChild(0);
        Vector2 curCordinate = new Vector2();

        for (int i = 0; i < tileY; i++)
        {
            curCordinate.y++;

            var newPos = firstTileTransform.position;

            newPos.z += yOffset;

            ChessTile tile = Instantiate(firstTileTransform.gameObject, newPos,
                firstTileTransform.rotation, transform).GetComponent<ChessTile>();
            tile.pos = curCordinate;

            yOffset += _YOffset;
        }

        yOffset = _YOffset;
        curCordinate.y = 0;

        for (int i = 0; i < tileX; i++)
        {
            curCordinate.x++;

            var newPos = firstTileTransform.position;

            newPos.x -= xOffset;

            GameObject currentGrid = Instantiate(firstTileTransform.gameObject, newPos,
                firstTileTransform.rotation, transform);

            ChessTile tile = currentGrid.GetComponent<ChessTile>();
            tile.pos = curCordinate;

            xOffset += _XOffset;

            for (int ii = 0; ii < tileY; ii++)
            {
                curCordinate.y++;

                var newPosz = currentGrid.transform.position;

                newPosz.z += yOffset;

                ChessTile tiled = Instantiate(currentGrid, newPosz,
                    currentGrid.transform.rotation, transform).GetComponent<ChessTile>();
                tiled.pos = curCordinate;

                yOffset += _YOffset;
            }

            yOffset = _YOffset;
            curCordinate.y = 0;
        }
    }
}
