using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieSpawn : MonoBehaviour
{
    [Header("ZombieSpawn Var")]
    public GameObject zombiePrefab;
    public Transform zombieSpawnPosition;
   // public GameObject dangerZone;
    private float repeatCycle = 1f;
   

    private void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player"){
            
            {
                InvokeRepeating("EnemySpawner", 1f, repeatCycle);
                Destroy(gameObject, 2f);
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
            
        }
    }

    void EnemySpawner()
    {
       
            Instantiate(zombiePrefab, zombieSpawnPosition.position, zombieSpawnPosition.rotation);
        
    }
}
