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
        Shotgun,
        Axe

    }

    [SerializeField] GameObject axeModel;

    [Header("Controller")]
    [SerializeField] CharacterController controller;

    [Space(10)]
    [Header("Player Stats")]
    [Space(10)]

    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] float walkingSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpHeight;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] float rageTimeLength;
    [Range(0.0f, 1.0f)][SerializeField] float rageDamageReduction;
    float rageMeter;
    float rageTimer = 0;
    float rageSpeed;
    float speedOriginal;
    bool isSprinting;
    bool isRaging;

    [Space(10)]
    [Header("Shooting System")]
    [Space(10)]

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Weapon weapon;
    float fireRate;
    float bloomMod;
    int damage;
    int rageDamage;
    int damageOriginal;
    float rageMeterIncrement;
    int fireDistance;
    int bullets;
    bool isAutomatic = false;
    float fireTimer;

    [Space(10)]
    [Header("Crounch")]
    [Space(10)]

    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchYScale;
    [SerializeField] float startYScale;

    [Category("Melee System")]
    float swingDistance;
    float swingRate;
    float swingTimer;


    [Category("Dash")]
    [SerializeField] float dashDistance;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;
    float dashTimer;
    [SerializeField] LayerMask dashWallCollision; // Layer Mask so player doesn't dash through walls

    [Space(10)]
    [Header("Player Materials")]
    [Space(10)]

    [SerializeField] Material hurtMaterial;

    Vector3 jumpVec;
    Vector3 moveDir;
    int jumpCount;
    int healthMax;

    void Start()
    {
        healthMax = health;
        dashTimer = dashCooldown;
        speedOriginal = speed;
        damageOriginal = damage;

        updatePlayerUI();
        GameManager.instance.updateGameGoal(1);

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
                    rageMeterIncrement = 1000;
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
                    rageMeterIncrement = 5;
                    break;
                }

            case Weapon.Shotgun:
                {
                    fireDistance = 20;
                    fireRate = 0;
                    damage = 8;
                    bullets = 6;
                    bloomMod = 0.1f;
                    rageMeterIncrement = 8;
                    break;
                }

            case Weapon.Axe:
                {
                    swingDistance = 5;
                    swingRate = 0;
                    damage = 30;
                    bloomMod = 0.1f;

                    break;
                }
        
              

            default:
                break;
        }

        rageSpeed = speed * 1.5f;
        rageDamage = (int)(damage * 1.5f);
    }

    void Update()
    {
        
        Movement();
        sprint();
        updatePlayerUIDash();
        crouch();
    }

    void Movement()
    {
        GetNumpadInput();
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

        Rage();

        // Shooting System Based On If The Weapon Is Semi Auto Or Full Auto
        
        if(DetermineWeaponType() == "Ranged")
        {
            if ((isAutomatic && Input.GetButton("Fire1") && fireTimer >= fireRate) || (!isAutomatic && Input.GetButtonDown("Fire1")))
            {
                Shoot();
            }
        }
        else
        {

            if ((Input.GetButtonDown("Fire1") && swingTimer >= swingRate))
            {
                Swing();
            }
            
        }
            
        
        
    }

    void Rage()
    {
        if (Input.GetButtonDown("Rage") && !isRaging && rageMeter == 1000)
        {
            RageAbilityStart();
        }

        if (isRaging)
        {
            rageTimer += Time.deltaTime;
            if (rageTimer >= rageTimeLength)
            {
                RageAbilityEnd();
            }
        }
    }

    void RageAbilityStart()
    {
        isRaging = true;
        speed = rageSpeed;
        damage = rageDamage;
    }

    void RageAbilityEnd()
    {
        isRaging = false;
        rageMeter = 0;
        rageTimer = 0;
        speed = speedOriginal;
        damage = damageOriginal;

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            jumpVec.y = jumpHeight;
        }
    }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            speed = crouchSpeed;
        }
        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            speed = walkingSpeed;
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

            transform.position = Vector3.Lerp(start, end, time / dashDuration);
            yield return null;
        }
    }

    void Swing()
    {
        RaycastHit hit;

        swingTimer = 0;

        float rangeX = Random.Range(-bloomMod, bloomMod);
        float rangeY = Random.Range(-bloomMod, bloomMod);

        if (Physics.Raycast(Camera.main.transform.position,
                                new Vector3(Camera.main.transform.forward.x + rangeX,
                                            Camera.main.transform.forward.y + rangeY,
                                            Camera.main.transform.forward.z),
                                out hit,
                                swingDistance,
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
                    AddRage(rageMeterIncrement);
                    dmg.TakeDamage(damage);
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= isRaging ? (int)(rageDamageReduction * amount) : amount;

        updatePlayerUI();
        StartCoroutine(FlashDamage());

        if (health <= 0)
        {
            // Lose proc
            GameManager.instance.updateToLoseScreen();
        }
    }

    public void updatePlayerUI()
    {
        GameManager.instance.PlayerHealth.fillAmount = (float)health / healthMax;
    }

    public void updatePlayerUIDash()
    {
        GameManager.instance.PlayerDash.fillAmount = dashTimer / (float)dashCooldown;
    }

    public void updatePlayerUIRage()
    {
        // uncomment this when the rage meter is added to the GameManager.
        //GameManager.instance.RageMeter.fillAmount = rageMeter / 1000;
    }

    IEnumerator FlashDamage()
    {
        GameManager.instance.PlayerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.PlayerDamageScreen.SetActive(false);
    }


    public void AddRage(float amount)
    {
        rageMeter += amount;
        rageMeter = Mathf.Clamp(rageMeter, 0, 1000);
        updatePlayerUIRage();
    }

    public string DetermineWeaponType()
    {
        if(weapon == Weapon.Pistol || weapon == Weapon.Shotgun || weapon == Weapon.AssaultRifle)
        {
            return "Ranged";
        }
        else
        {
            return "Melee";
        }

    }

    void SwitchWeapon(int weaponID) // uses a weapon id to switch the current weapon to a hard coded weapon slot.
    {
        switch (weaponID)
        {
            case 1: // pistol
                {
                    weapon = Weapon.Pistol;
                    break;
                }

            case 2: // Assault Rifle
                {
                    weapon = Weapon.AssaultRifle;
                    break;
                }

            case 3: // Shotgun
                {
                    weapon = Weapon.Shotgun;
                    break;
                }

            case 4: //Axe
                {
                    weapon = Weapon.Axe;
                    axeModel.gameObject.SetActive(true);
                    break;
                }



            default:
                break;
        }

    }

    void GetNumpadInput()
    {
        if(Input.GetButtonDown("Weapon1"))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetButtonDown("Weapon2"))
        {
            SwitchWeapon(2);
        }
        else if (Input.GetButtonDown("Weapon3"))
        {
            SwitchWeapon(3);
        }
        else if (Input.GetButtonDown("Weapon4"))
        {
            SwitchWeapon(4);
        }
    }


}

