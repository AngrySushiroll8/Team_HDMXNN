using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    // Weapon List
    enum Weapon
    {
        Pistol,
        AssaultRifle,
        Shotgun
    }

    [Category("Controller")]
    [SerializeField] CharacterController controller;

    [Category("Player Stats")]
    [SerializeField] int health;
    [SerializeField] int speed;
    [SerializeField] int jumpHeight;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;

    [Category("Shooting System")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Weapon weapon;
    float fireRate;
    float bloomMod;
    int damage;
    int fireDistance;
    int bullets;
    bool isAutomatic = false;
    float fireTimer;


    [Category("Player Materials")]
    [SerializeField] Material hurtMaterial;

    Vector3 jumpVec;
    Vector3 moveDir;
    int jumpCount;
    int healthMax;

    void Start()
    {
        healthMax = health;

        updatePlayerUI();

        // Sets Weapon Values Based On Weapon Type
        switch (weapon)
        {
            case Weapon.Pistol:
                {
                    fireDistance = 40;
                    fireRate = 0;
                    damage = 20;
                    bullets = 1;
                    bloomMod = 0.01f;
                    break;
                }

            case Weapon.AssaultRifle:
                {
                    isAutomatic = true;
                    fireDistance = 60;
                    fireRate = 0.25f;
                    damage = 30;
                    bullets = 1;
                    bloomMod = 0.015f;
                    break;
                }

            case Weapon.Shotgun:
                {
                    fireDistance = 20;
                    fireRate = 0;
                    damage = 50;
                    bullets = 6;
                    bloomMod = 0.1f;
                    break;
                }

            default:
                break;
        }
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        fireTimer += Time.deltaTime;

        // Gravity System
        if (controller.isGrounded)
        {
            jumpCount = 0;
            jumpVec = Vector3.zero;
        }
        else
        {
            jumpVec.y -= gravity * Time.deltaTime;
        }

        // WASD Movement
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        // Jump Movement
        Jump();
        controller.Move(jumpVec * Time.deltaTime);

        // Shooting System Based On If The Weapon Is Semi Auto Or Full Auto
        if ((isAutomatic && Input.GetButton("Fire1") && fireTimer >= fireRate) || (!isAutomatic && Input.GetButtonDown("Fire1")))
        {
            Shoot();
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            jumpVec.y = jumpHeight;
        }
    }

    void Shoot()
    {
        fireTimer = 0;

        for (int bulletIndex = 0; bulletIndex < bullets; bulletIndex++)
        {
            RaycastHit hit;
            float rangeX = Random.Range(-bloomMod, bloomMod);
            float rangeY = Random.Range(-bloomMod, bloomMod);

            // Debug Ray For Testing Weapons
            int rayLength = 10;
            int rayDuration = 5;
            Debug.DrawRay(Camera.main.transform.position,
                          new Vector3(Camera.main.transform.forward.x + rangeX,
                                    Camera.main.transform.forward.y + rangeY,
                                    Camera.main.transform.forward.z) * rayLength,
                          Color.red,
                          rayDuration);

            // Raycast For Shooting
            if (Physics.Raycast(Camera.main.transform.position,
                                new Vector3(Camera.main.transform.forward.x + rangeX,
                                            Camera.main.transform.forward.y + rangeY,
                                            Camera.main.transform.forward.z),
                                out hit,
                                fireDistance,
                                ~ignoreLayer))
            {
                Debug.Log("HIT! | " + hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.TakeDamage(damage);
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        updatePlayerUI();

        if (health <= 0)
        {
            // Lose
            Destroy(gameObject);
        }
        else
        {
            //StartCoroutine(FlashDamage());
        }
    }

    public void updatePlayerUI()
    {
        GameManager.instance.PlayerHealth.fillAmount = (float)health / healthMax;
    }

    // IEnumerator FlashDamage()
    // {
    //     GameManager.instance.playerDamageScreen.SetActive(true);
        
    //     GameManager.instance.playerDamageScreen.GetComponent<Image>().color = color;

    //     yield return new WaitForSeconds(0.1f);

    //     GameManager.instance.playerDamageScreen.SetActive(false);
    // }
}
