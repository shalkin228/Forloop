using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessGrid : MonoBehaviour
{
    [SerializeField] private float _XOffset, _YOffset;

    private void Start()
    {
        SpawnTiles(7, 7, _XOffset, _YOffset);
    }

    private void SpawnTiles(float tileX, float tileY, float xOffset, float yOffset)
    {
        var firstTileTransform = transform.GetChild(0);

        for (int i = 0; i < tileY; i++)
        {
            var newPos = firstTileTransform.position;

            newPos.z += yOffset;

            Instantiate(firstTileTransform.gameObject, newPos,
                firstTileTransform.rotation, transform);

            yOffset += _YOffset;
        }

        yOffset = _YOffset;

        for (int i = 0; i < tileX; i++)
        {
            var newPos = firstTileTransform.position;

            newPos.x -= xOffset;

            GameObject currentGrid = Instantiate(firstTileTransform.gameObject, newPos,
                firstTileTransform.rotation, transform);

            xOffset += _XOffset;

            for (int ii = 0; ii < tileY; ii++)
            {
                var newPosz = currentGrid.transform.position;

                newPosz.z += yOffset;

                Instantiate(currentGrid, newPosz,
                    currentGrid.transform.rotation, transform);

                yOffset += _YOffset;
            }

            yOffset = _YOffset;
        }
    }
}
