using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
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
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpHeight;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    bool isSprinting;

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

    [Category("Dash")]
    [SerializeField] float dashDistance;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;
    float dashTimer;
    [SerializeField] LayerMask dashWallCollision; // Layer Mask so player doesn't dash through walls

    [Category("Player Materials")]
    [SerializeField] Material hurtMaterial;

    Vector3 jumpVec;
    Vector3 moveDir;
    int jumpCount;
    int healthMax;

    void Start()
    {
        healthMax = health;
        dashTimer = dashCooldown;

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
        sprint();
        updatePlayerUIDash();
    }

    void Movement()
    {
        fireTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;

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

        // Dash ability
        if (Input.GetButton("Dash") && dashTimer >= dashCooldown)
        {
            StartCoroutine(dash());
        }

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

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator dash()
    {
        dashTimer = 0;
        Vector3 start = transform.position;
        Vector3 end = (transform.position + (transform.forward * dashDistance));
        float time = 0f;

        while (time < dashDuration)
        {
            if (Physics.BoxCast(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, 0.1f),
                transform.forward / 10, transform.rotation, 1, dashWallCollision))
            {
                break;
            }

            time += Time.deltaTime;

            //if(dashTimer <= dashCooldown)
            //{
            //    dashTimer += time;
            //    updatePlayerUI();
            //}
            transform.position = Vector3.Lerp(start, end, time / dashDuration);     
            yield return null;
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
        StartCoroutine(FlashDamage());

        if (health <= 0)
        {
            // Lose
            Destroy(gameObject);
        }
    }

    public void updatePlayerUI()
    {
        GameManager.instance.PlayerHealth.fillAmount = (float)health / healthMax;
    }

    IEnumerator FlashDamage()
    {
         GameManager.instance.PlayerDamageScreen.SetActive(true);

         yield return new WaitForSeconds(0.1f);

         GameManager.instance.PlayerDamageScreen.SetActive(false);
    }

    public void updatePlayerUIDash()
    {
        GameManager.instance.PlayerDash.fillAmount = dashTimer / (float)dashCooldown;
    }
}

