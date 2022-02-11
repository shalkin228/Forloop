using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint, _enemySpawnPoint;

    public void Setup(GameObject enemy, GameObject player)
    {
        var instantiatedPlayer = Instantiate(player);
        var instantiatedEnemy = Instantiate(enemy);

        instantiatedEnemy.transform.SetParent(transform);
        instantiatedPlayer.transform.SetParent(transform);

        instantiatedPlayer.transform.position = _playerSpawnPoint.position;
        instantiatedEnemy.transform.position = _enemySpawnPoint.position;
    }
}
