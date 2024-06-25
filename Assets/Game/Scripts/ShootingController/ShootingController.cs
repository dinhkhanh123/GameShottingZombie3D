using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerMovement playerMovement;
    public GameObject goreEffect;

    [Header("Shooting Var")]
    public Transform firePoint;
    public float fireRate = 0f;
    public float fireRange = 100f;
    public float fireDamage = 15f;
    private float nextFireTime = 0f;

    [Header("Reloading")]
    public int maxAmmo = 30;
    private int currentAmount;
    public float reloadTime = 1.5f;
    public TextMeshProUGUI countBullet;

    [Header("Shooting Flags")]
    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;
    public bool isReloading = false;
    public bool isScopeInput;

    [Header("Sound Effects")]
    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;
    public AudioClip reloadingSoundClip;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        currentAmount = maxAmmo;
    }

     void Update()
    {
        if(currentAmount== maxAmmo)
        {
            inputManager.reloadInput = false;
        }
        if(isReloading || playerMovement.isSprinting)
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            return;
        }
        isWalking = playerMovement.isMoving;
        isShootingInput = inputManager.fireInput;
        isScopeInput = inputManager.scopeInput;

        if (isShootingInput && isWalking)
        {
            if(Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();

                animator.SetBool("ShootWalk", true);
            }
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement",true);
            isShooting = true;
        }else if (isShootingInput)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
                
            }
            animator.SetBool("Shoot", true);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            isShooting = true;
        }else if (isScopeInput)
        {
            animator.SetBool("Shoot", true);
            animator.SetBool("ShootingMovement", true);
            animator.SetBool("ShootWalk", false);
            isShooting = false;
        }
        else
        {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            isShooting = false;
        }

        if(inputManager.reloadInput && currentAmount < maxAmmo)
        {
            Reload();
            
        }
        countBullet.text = "Ammo: " + currentAmount.ToString();

    }

    private void Shoot()
    {

        if (playerMovement.isJumping)
            return;

        if (currentAmount > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Enemy zombie = hit.transform.GetComponent<Enemy>(); 
                Zombie zombie1 = hit.transform.GetComponent<Zombie>();  
                //Debug.Log(hit.transform.name);

                if(zombie != null)
                {
                   
                    zombie.zombieHitDamage(fireDamage);
                    GameObject goreEffectGo = Instantiate(goreEffect,hit.point,Quaternion.LookRotation(hit.normal));
                    Destroy(goreEffectGo,1f);
                  

                }
                if(zombie1 != null)
                {
                    zombie1.zombieHitDamage(fireDamage);
                    GameObject goreEffectGo = Instantiate(goreEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(goreEffectGo, 1f);
                }
            }
            muzzleFlash.Play();
            soundAudioSource.PlayOneShot(shootingSoundClip);
            currentAmount--;
            


        }
        else
        {
            Reload();
           
        }
    }

    private void Reload()
    {
        inputManager.scopeInput = false;
      

        if (!isReloading && currentAmount < maxAmmo)
        {
            if(isShooting && isWalking)
            {
                animator.SetTrigger("ShootReload");
            }
            else
            {
                animator.SetTrigger("Reload");
            }
            isReloading = true;
            soundAudioSource.PlayOneShot(reloadingSoundClip);
            Invoke("FinishReloading", reloadTime);
            

        }

    }

    private void FinishReloading()
    {
        currentAmount = maxAmmo;
        isReloading = false;
    }
}
