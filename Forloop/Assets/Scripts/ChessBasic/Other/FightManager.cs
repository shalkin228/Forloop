using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private static EnemyBase _enemy;

    public static void StartFight(GameObject location, GameObject player, 
        GameObject enemy, EnemyBase chessEnemy)
    {
        Instantiate(location).GetComponent<LocationManager>().Setup
            (enemy, 
            player);

        Camera.main.transform.GetChild(0).gameObject.SetActive(true);

        _enemy = chessEnemy;
    }

    public static void StopFight()
    {
        TabletManager.instance.CloseTablet();


    }
}
