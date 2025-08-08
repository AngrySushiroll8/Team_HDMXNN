using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI_Base : MonoBehaviour, IDamage
{

    [SerializeField] protected Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected int HP;
    [SerializeField] protected int faceTargetSpeed;

    protected Transform player;

    protected Color colorOrig;

    protected bool playerInTrigger;

    protected Vector3 playerDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        player = GameManager.instance.player.transform;
        colorOrig = model.material.color;



    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }

    }
}
