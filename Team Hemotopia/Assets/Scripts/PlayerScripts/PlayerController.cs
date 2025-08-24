using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
//using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;



public class PlayerController : MonoBehaviour, IDamage, IPickup
{
    // Weapon List
    enum Weapon
    {
        Pistol,
        AssaultRifle,
        Shotgun,
        Axe

    }



    [Space(10)]
    [Header("Models")]
    [Space(10)]

    [SerializeField] public List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    int gunListPos;

    [Space(10)]
    [Header("Controller")]
    [Space(10)]

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
    [SerializeField] float rageMax;
    [Range(0.0f, 1.0f)][SerializeField] float rageDamageReduction;
    float rageMeter;
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
    bool isCrouching;

    [Space(10)]
    [Category("Melee System")]
    [Space(10)]

    float swingDistance;
    float swingRate;
    float swingTimer;


    [Space(10)]
    [Header("Dash")]
    [Space(10)]

    [SerializeField] float dashDistance;
    [SerializeField] float dashDuration;
    [SerializeField] int dashCooldown;
    float dashTimer;
    [SerializeField] LayerMask wallCollision; // Layer Mask so player doesn't dash through walls

    [Space(10)]
    [Header("Slide")]
    [Space(10)]
    [SerializeField] float slideDistance;
    [SerializeField] float slideDuration;
    [SerializeField] int slideCooldown;
    float slideTimer;


    [Space(10)]
    [Header("Player Materials")]
    [Space(10)]

    [SerializeField] Material hurtMaterial;

    public Vector3 jumpVec;
    Vector3 moveDir;
    int jumpCount;
    int healthMax;

    public bool jumpPadded;
    float gravityDelay;

    //Testing the timer
    bool doubleJumpIsActive;
    bool speedBoostIsActive;

    void Start()
    {
        healthMax = health;
        dashTimer = dashCooldown;
        slideTimer = slideCooldown;
        speedOriginal = speed;


        updatePlayerUI();


       

        damageOriginal = damage;
        rageSpeed = speed * 1.5f;
        rageDamage = (int)(damage * 1.5f);
    }

    void Update()
    {
        if(!GameManager.instance.isPaused)
        {
            Movement();
        }
        
        updatePlayerUIDash();

        if(doubleJumpIsActive || speedBoostIsActive)
        {
            if (doubleJumpIsActive)
            {
                GameManager.instance.doubleJumpTimerUpdate();

                if (GameManager.instance.doubleJumpTimerCount <= 0)
                {
                    GameManager.instance.activePowerUp.SetActive(false);
                    GameManager.instance.activePowerUp = null;
                    GameManager.instance.doubleJumpTimerCount = 10;
                    doubleJumpIsActive = false;
                }
            }

            if(speedBoostIsActive)
            {
                GameManager.instance.speedBoostTimerUpdate();

                if (GameManager.instance.speedBoostTimerCount <= 0)
                {
                    GameManager.instance.activePowerUp.SetActive(false);
                    GameManager.instance.activePowerUp = null;
                    GameManager.instance.speedBoostTimerCount = 5;
                    speedBoostIsActive = false;
                }
            }
        }
    }

    void Movement()
    {
        
        fireTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
        slideTimer += Time.deltaTime;

        // Jump Pad
        if (jumpPadded)
        {
            gravityDelay += Time.deltaTime;
            if (gravityDelay > 0.3f)
            {
                jumpPadded = false;
                gravityDelay = 0;
            }
        }
        
        // Gravity System
        if (controller.isGrounded && !jumpPadded)
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
        if (Input.GetButton("Dash") && dashTimer >= dashCooldown && !isCrouching)
        {
            if (moveDir == new Vector3(0, 0, 0))
            {

            }
            else
            {
                StartCoroutine(dash());
            }
        }

        // Rage ability
        Rage();

        // Crouch
        crouch();

        // Sprint
        sprint();

        if (isCrouching) speed = crouchSpeed;

        // Slide
        if (Input.GetButton("Crouch") && isSprinting && slideTimer >= slideCooldown && controller.isGrounded)
        {
            StartCoroutine(slide());
            speed = crouchSpeed;
            isCrouching = true;
        }

      
            if ((isAutomatic && Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && fireTimer >= fireRate) 
            || (!isAutomatic && Input.GetButtonDown("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0))
            {
                Shoot();
            }
        
        SelectGun();

        Reload();

    }

    void Rage()
    {
        // Starts rage ability if the player hits the rage button, is not raging, and if the rage meter is full
        if (Input.GetButtonDown("Rage") && !isRaging && rageMeter == rageMax)
        {
            RageAbilityStart();
        }
        else
        {
            updatePlayerUIRage();
        }

        // Decreases the rage meter then update the UI and check if the rage is over
        if (isRaging)
        {
            DecreaseRage();
            updatePlayerUIRage();

            if (rageMeter <= 0)
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
        if (Input.GetButtonDown("Crouch") && !isSprinting)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            speed = crouchSpeed;
            isCrouching = true;
        }
        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            speed = walkingSpeed;
            isCrouching = false;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isCrouching)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed = walkingSpeed;
            isSprinting = false;
        }
    }
    IEnumerator slide()
    {
        slideTimer = 0;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z);
        Vector3 start = transform.position;
        Vector3 end = (transform.position + (transform.forward * dashDistance));
        float time = 0f;

        while (time < slideDuration)
        {
            if (Physics.BoxCast(transform.position, new Vector3(transform.localScale.x - 0.2f, transform.localScale.y, 0.1f),
                transform.forward, transform.rotation, 1, wallCollision))
            {
                break;
            }

            time += Time.deltaTime;

            transform.position = Vector3.Lerp(start, end, time / slideDuration);
            yield return null;
        }
    }
    IEnumerator dash()
    {
        dashTimer = 0;
        Vector3 start = transform.position;
        Vector3 end = (transform.position + (moveDir * dashDistance));
        
        float time = 0f;

        while (time < dashDuration)
        {
            if (Physics.BoxCast(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, 0.1f),
                moveDir, Quaternion.LookRotation(moveDir, Vector3.up), 1, wallCollision))
            {
                break;
            }

            time += Time.deltaTime;

            transform.position = Vector3.Lerp(start, end, time / dashDuration);
            yield return null;
        }
    }


    void Shoot()
    {
        fireTimer = 0;
        gunList[gunListPos].ammoCur--;

        Dictionary<IDamage, int> damages = new Dictionary<IDamage, int>();

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
                //Debug.Log("HIT! | " + hit.collider.name);
                Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    AddRage(rageMeterIncrement);
                    if (damages.ContainsKey(dmg))
                    {
                        damages[dmg] += damage;
                    }
                    else
                    {
                        damages.Add(dmg, damage);
                    }
                }
            }
        }
        foreach (KeyValuePair<IDamage, int> entry in damages)
        {
            entry.Key.TakeDamage(entry.Value);
        }
    }

    void Reload()
    {
        if(Input.GetButtonDown("Reload"))
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
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
        GameManager.instance.RageMeter.fillAmount = rageMeter / 1000;
    }

    IEnumerator FlashDamage()
    {
        GameManager.instance.PlayerDamageScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.PlayerDamageScreen.SetActive(false);
    }


    public void AddRage(float amount)
    {
        // Adds rage if the player is not raging
        if (!isRaging)
        {
            rageMeter += amount;
            rageMeter = Mathf.Clamp(rageMeter, 0, rageMax);
            updatePlayerUIRage();
        }
    }

    public string DetermineWeaponType()
    {
        if (weapon == Weapon.Pistol || weapon == Weapon.Shotgun || weapon == Weapon.AssaultRifle)
        {
            return "Ranged";
        }
        else
        {
            return "Melee";
        }

    }

    public void HealPlayer(int amount)
    {
        health += amount;

        updatePlayerUI();
        StartCoroutine(FlashHeal());

        if (health > healthMax)
        {
            health = healthMax; // does not allow for healing above max health

        }
    }

    IEnumerator FlashHeal()
    {
        GameManager.instance.PlayerHealScreen.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        GameManager.instance.PlayerHealScreen.SetActive(false);
    }

    IEnumerator DoubleJumpEnum(float effectDuration)
    {
        doubleJumpIsActive = true;
        jumpMax = 2;
        GameManager.instance.activePowerUp = GameManager.instance.doubleJumpText;
        GameManager.instance.activePowerUp.SetActive(true);
        GameManager.instance.doubleJumpTimerCount = effectDuration;
        yield return new WaitForSeconds(effectDuration);
        jumpMax = 1;
    }
    public void DoubleJump(float effectDuration)
    {
        StartCoroutine(DoubleJumpEnum(effectDuration));
    }

    IEnumerator SpeedBoostEnum(float speedBoostMulti, float effectDuration)
    {
        speedBoostIsActive = true;
        speed *= speedBoostMulti;
        GameManager.instance.activePowerUp = GameManager.instance.speedBoostText;
        GameManager.instance.activePowerUp.SetActive(true);
        GameManager.instance.speedBoostTimerCount = effectDuration;
        yield return new WaitForSeconds(effectDuration);
        speed = speedOriginal;
    }

    public void SpeedBoost(float speedBoostMulti, float effectDuration)
    {
        StartCoroutine(SpeedBoostEnum(speedBoostMulti, effectDuration));
    }
    void SwitchWeapon(int weaponID) // uses a weapon id to switch the current weapon to a hard coded weapon slot.
    {
        switch (weaponID)
        {
            case 1: // pistol
                {
                    weapon = Weapon.Pistol;
                    HideAllWeapons();
                    //pistolModel.gameObject.SetActive(true);

                    //GameManager.instance.PistolIcon.SetActive(true);
                    GameManager.instance.ActiveReticle.SetActive(false);
                    GameManager.instance.ActiveReticle = null;
                   
                    GameManager.instance.ActiveReticle = GameManager.instance.PistolReticle;
                    GameManager.instance.ActiveReticle.SetActive(true);

                    break;
                }

            case 2: // Assault Rifle
                {
                    weapon = Weapon.AssaultRifle;
                    HideAllWeapons();
                    //assaultRifleModel.gameObject.SetActive(true);
                    //GameManager.instance.ARIcon.SetActive(true);
                    GameManager.instance.ActiveReticle.SetActive(false);
                    GameManager.instance.ActiveReticle = null;
                    
                    GameManager.instance.ActiveReticle = GameManager.instance.ARReticle;
                    GameManager.instance.ActiveReticle.SetActive(true);

                    break;
                }

            case 3: // Shotgun
                {
                    weapon = Weapon.Shotgun;
                    HideAllWeapons();
                    //shotgunModel.gameObject.SetActive(true);
                    //GameManager.instance.ShotgunIcon.SetActive(true);
                    GameManager.instance.ActiveReticle.SetActive(false);
                    GameManager.instance.ActiveReticle = null;
                   
                    GameManager.instance.ActiveReticle = GameManager.instance.ShotgunReticle;
                    GameManager.instance.ActiveReticle.SetActive(true);

                    break;
                }

            case 4: //Axe
                {
                    weapon = Weapon.Axe;
                    HideAllWeapons();
                    //axeModel.gameObject.SetActive(true);
                    //GameManager.instance.AxeIcon.SetActive(true);
                    GameManager.instance.ActiveReticle.SetActive(false);
                    GameManager.instance.ActiveReticle = null;

                    GameManager.instance.ActiveReticle = GameManager.instance.AxeReticle;
                    GameManager.instance.ActiveReticle.SetActive(true);

                    break;
                }

            default:
                break;
        }

    }

    void GetNumpadInput()
    {
        if (Input.GetButtonDown("Weapon1"))
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

    void SwitchCaseWeapon(Weapon weapon)
    {
        switch (weapon)
        {
            case Weapon.Pistol:
                {
                    isAutomatic = false;
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
                    damage = 10;
                    bullets = 1;
                    bloomMod = 0.015f;
                    rageMeterIncrement = 10;
                    break;
                }

            case Weapon.Shotgun:
                {
                    isAutomatic = false;
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
                    rageMeterIncrement = 30;
                    break;
                }

            default:
                break;
        }
    }

    void HideAllWeapons() // sets visibility of all weapons to false
    {
        //pistolModel.gameObject.SetActive(false);
        //assaultRifleModel.gameObject.SetActive(false);
        //shotgunModel.gameObject.SetActive(false);
        //axeModel.gameObject.SetActive(false);
    }

    void DecreaseRage()
    {
        rageMeter -= (rageMax / rageTimeLength) * Time.deltaTime;
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);

        gunListPos = gunList.Count - 1;

        ChangeGun();

    }

    void ChangeGun()
    {
        isAutomatic = gunList[gunListPos].isAutomatic;
        fireDistance = gunList[gunListPos].fireDist;
        fireRate = gunList[gunListPos].fireRate;
        bullets = gunList[gunListPos].bullets;
        bloomMod = gunList[gunListPos].bloomMod;
        rageMeterIncrement = gunList[gunListPos].rageMeterIncrement;
        damage = gunList[gunListPos].damage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();
        }
    }
}

