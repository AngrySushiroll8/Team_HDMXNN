using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Category("Controller")]
    [SerializeField] CharacterController controller;

    [Category("Player Stats")]
    [SerializeField] int health;
    [SerializeField] int speed;
    [SerializeField] int jumpHeight;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int damage;

    [Category("Shooting System")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] float fireTimer;
    [SerializeField] int fireRate;
    [SerializeField] int fireDistance;


    [Category("Player Materials")]
    [SerializeField] Material hurtMaterial;

    Vector3 jumpVec;
    Vector3 moveDir;
    int jumpCount;
    int healthMax;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthMax = health;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        fireTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            jumpVec = Vector3.zero;
        }
        else
        {
            jumpVec.y -= gravity * Time.deltaTime;
        }

        moveDir = (Input.GetAxis("Horizontal") * Vector3.right) + (Input.GetAxis("Vertical") * Vector3.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        Jump();
        controller.Move(jumpVec * Time.deltaTime);

        if (Input.GetButton("Fire1") && fireTimer >= fireRate)
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

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, fireDistance, ~ignoreLayer))
        {
            Debug.Log("HIT! | " + hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

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

    // IEnumerator FlashDamage()
    // {
    //     GameManager.instance.playerDamageScreen.SetActive(true);
        
    //     GameManager.instance.playerDamageScreen.GetComponent<Image>().color = color;

    //     yield return new WaitForSeconds(0.1f);

    //     GameManager.instance.playerDamageScreen.SetActive(false);
    // }
}
