using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public delegate void ZombieKilled();
    public static event ZombieKilled OnZombieKilled;
    public GameObject[] itemMode;

    private GameObject player;

    [Header("Zombie Health and Damege")]
    const float zombieHealth = 100f;
    public float currentHealth;
    public float giveDamage = 5f;
    

    public Slider healthSlider;
    public GameObject enemyUI;


    [Header("Zombie Things")]
    [SerializeField] NavMeshAgent zombieAgent;
    private PlayerMovement playerMovement;
    //public Transform LookPoint;
    public Camera AttackingRaycastArea;
    //public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Zombie Guarding Var")]
    public float zombieSpeed;

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
    private bool isDead = false;

    void Awake()
    {
        currentHealth = zombieHealth;
        zombieAgent = GetComponent<NavMeshAgent>();
        playerMovement = GetComponent<PlayerMovement>();

        

        healthSlider.minValue = 0f;
        healthSlider.maxValue = zombieHealth;
        healthSlider.value = currentHealth;
    }

    void Start()
    {
        // Tìm Player b?ng tag "Player"
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

     void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer);
        playerInattackingRadius = Physics.CheckSphere(transform.position, attackingRadius, PlayerLayer);

        if (player == null)
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Runing", false);
            anim.SetBool("Attacking", false);
            anim.SetBool("Died", false);
        }
        if (!playerInvisionRadius && !playerInattackingRadius) Idle();

        if (playerInvisionRadius && !playerInattackingRadius) PursuePlayer();

        if (playerInvisionRadius && playerInattackingRadius) AttackPlayer();

     
    }


    private void Idle()
    {
       // zombieAgent.SetDestination(playerBody.position);
    }
    // follow player
    private void PursuePlayer()
    {
        if (player != null)
        {

            if (zombieAgent.SetDestination(player.transform.position))
            {
                anim.SetBool("Idle", false);
                anim.SetBool("Runing", true);
                anim.SetBool("Attacking", false);
                anim.SetBool("Died", false);
            }
            else
            {
                anim.SetBool("Idle", false);
                anim.SetBool("Runing", false);
                anim.SetBool("Attacking", false);
                anim.SetBool("Died", true);
            }
        }

    }
    private void AttackPlayer()
    {
   
        //transform.LookAt(LookPoint);
        if (!previouslyAtttack)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(AttackingRaycastArea.transform.position, AttackingRaycastArea.transform.forward, out hitInfo, attackingRadius))
            {
               // Debug.Log("Attacking : " + hitInfo.transform.name);

                PlayerMovement playerBody = hitInfo.transform.GetComponent<PlayerMovement>();
                if (playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage);
                }

             
                    anim.SetBool("Idle", false);
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
        if (!isDead) 
        {
            currentHealth -= takeDamage;
            healthSlider.value = currentHealth;

            if (currentHealth <= 0)
            {
                DiedAnim();
                zombieDie();
            }
        }
    }

    public void zombieDie()
    {
        if (!isDead)
        {
            isDead = true;
            zombieSpeed = 0;
            attackingRadius = 0;
            visionRadius = 0;
            playerInvisionRadius = false;
            playerInattackingRadius = false;

            UIManager.instance.killCount++;
            UIManager.instance.UpdateKillCounter();

            Destroy(gameObject);
            DropItem();

            if (OnZombieKilled != null)
            {
                OnZombieKilled();
            }

        }
    }
    private void DiedAnim()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Runing", false);
        anim.SetBool("Attacking", false);
        anim.SetBool("Died", true);
    }

     void DropItem()
    {
        Vector3 position = transform.position;
        int numberItem = Random.Range(0, itemMode.Length - 1);
        GameObject item = Instantiate(itemMode[numberItem],position, Quaternion.identity);
        item.SetActive(true);
        Destroy(item, 10f);
    }
}
