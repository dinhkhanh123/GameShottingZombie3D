using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Zombie Health and Damege")]
    const float zombieHealth = 100f;
    public float currentHealth;
    public float giveDamage = 5f;


    [Header("Zombie Things")]
     public NavMeshAgent zombieAgent; 
    public Transform LookPoint;
    public Camera AttackingRaycastArea;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public GameObject[] walkPoints;
    int currentZombiePosition = 0;
    public float zombieSpeed;
    float walkingpointRadius = 2f;

    [Header("Zombie Attacking Var")]
    public float timeBtwAttack;
    bool previouslyAtttack;

    [Header("Zombie Animation")]
    public Animator anim;

    [Header("Zombie mood/states")]
    public float visionRadius;
    public float attackingRadius;
    public bool playerInvisionRadius;
    public bool playerInattackingRadius;


     void Awake()
    {
        currentHealth=zombieHealth;
        zombieAgent = GetComponent<NavMeshAgent>();
      
    }

     void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position,visionRadius,PlayerLayer);
        playerInattackingRadius = Physics.CheckSphere(transform.position,attackingRadius,PlayerLayer);

        if (!playerInvisionRadius && !playerInattackingRadius) Guard();
        if (playerInvisionRadius && !playerInattackingRadius) PursuePlayer();
        if (playerInvisionRadius && playerInattackingRadius ) AttackPlayer();
        if (currentHealth <= 0)
        {
            DiedAnim();
        }
    }



    private void Guard()
    {
        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position,transform.position) < walkingpointRadius)
        {
            currentZombiePosition = Random.Range(0,walkPoints.Length); 
            if(currentZombiePosition >= walkPoints.Length)
            {
                currentZombiePosition = 0;
            }
        }
        anim.SetBool("Walking", true);
        anim.SetBool("Runing", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("Died", false);

        transform.position = Vector3.MoveTowards(transform.position, walkPoints[currentZombiePosition].transform.position, Time.deltaTime * zombieSpeed);
        // change zombie facing
        transform.LookAt(walkPoints[currentZombiePosition].transform.position);   

    }

    private void PursuePlayer()
    {

        //Vector3 directionToPlayer = (playerBody.position - transform.position).normalized;

        //Vector3 newPosition = transform.position + directionToPlayer * zombieSpeed * Time.deltaTime;
        //transform.position = newPosition;

        //transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (zombieAgent.SetDestination(playerBody.position))
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Runing", true);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", false);
        }
        else
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Runing", false);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", true);
        }

    }
    private void AttackPlayer()
    {
        transform.LookAt(LookPoint);
            if (!previouslyAtttack)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(AttackingRaycastArea.transform.position, AttackingRaycastArea.transform.forward, out hitInfo, attackingRadius))
                {
                    Debug.Log("Attacking : " + hitInfo.transform.name);

                    PlayerMovement playerBody = hitInfo.transform.GetComponent<PlayerMovement>();
                    if (playerBody != null)
                    {
                        playerBody.playerHitDamage(giveDamage);
                    }

                    anim.SetBool("Walking", false);
                    anim.SetBool("Runing", false);
                    anim.SetBool("Attacking", true);
                    anim.SetBool("Died", false);

                }
                previouslyAtttack = true;
                Invoke(nameof(ActiveAttacking), timeBtwAttack);
            }
      
       
    }

    private void ActiveAttacking()
    {
        previouslyAtttack = false;
    }

    public void zombieHitDamage(float takeDamage) 
    {
        currentHealth -= takeDamage;
        if(currentHealth <= 0)
        {
            zombieDie();
        }
    }

    private void zombieDie()
    {
        zombieSpeed = 0;
        attackingRadius = 0;
        visionRadius = 0;
        playerInvisionRadius = false;
        playerInattackingRadius = false;

        Destroy(gameObject, 2f);
    }

    private void DiedAnim()
    {
        anim.SetBool("Walking", false);
        anim.SetBool("Runing", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("Died", true);
    }
}
