using UnityEngine;

public class RotationTriggerEvents : MonoBehaviour
{
    [SerializeField] float xRot, yRot, zRot;
    private Transform rotationTarget;
    bool playerInTrigger;
    Vector3 playerMoveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationTarget = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
    }

    void FixedUpdate()
    {
        if (playerInTrigger)
        {
            Vector3 playerDirection = GameManager.instance.player.transform.position - transform.position;
            float angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
            GameManager.instance.player.transform.position = Vector3.Lerp(GameManager.instance.player.transform.position, GameManager.instance.player.transform.position + (angleToPlayer <= 90 ? transform.forward : -transform.forward), 0.5f * Time.deltaTime);
        }
    }

    void Rotation()
    {
        rotationTarget.Rotate(xRot * Time.deltaTime, yRot * Time.deltaTime, zRot * Time.deltaTime);

    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
