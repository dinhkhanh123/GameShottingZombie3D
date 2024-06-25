using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public Transform[] spawnPoint;
    public GameObject zombiePrefab;

    private bool canSpawnZombie = true;

    void Start()
    {
        SpawnNewZombie();
    }
     void OnEnable()
    {
        Zombie.OnZombieKilled += SpawnNewZombie;

    }
    void SpawnNewZombie()
    {
        
        if (canSpawnZombie && UIManager.instance.killCount < UIManager.instance.numKillLevel)
        {
            int randomIndex = Random.Range(0, spawnPoint.Length);
            if (spawnPoint[randomIndex] != null)
            {
                Instantiate(zombiePrefab, spawnPoint[randomIndex].position, Quaternion.identity);
            }
        }
    }

    public void StopSpawnZombie()
    {
        canSpawnZombie = false;
    }

}
