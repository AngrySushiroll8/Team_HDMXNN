using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] int health;
    [SerializeField] int speed;
    [SerializeField] int jumpHeight;
    [SerializeField] int gravity;
    [SerializeField] int jumpMax;

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
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            jumpVec.y = jumpHeight;
        }
    }

    public void TakeDamage(int amount)
    {

    }
}
