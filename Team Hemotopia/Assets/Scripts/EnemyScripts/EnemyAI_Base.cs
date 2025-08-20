using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI_Base : MonoBehaviour, IDamage
{

    [SerializeField] protected Renderer model;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected int HP;
    [SerializeField] protected int faceTargetSpeed;
    [SerializeField] protected int FOV;
    [SerializeField] protected float roamDistance;
    [SerializeField] protected float roamPauseTimer;

    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;
        [Range(0, 1)] public float chance = 1;
        public int minCount;
        public int maxCount;
    }

    [SerializeField] bool enableDrops = true;
    [SerializeField] DropItem[] drops;

    protected Transform player;

    protected Color colorOrig;

    protected bool playerInTrigger;

    protected Vector3 playerDir;

    protected float angleToPlayer;

    protected float roamTimer;

    protected float stoppingDistOrig;

    protected Vector3 startingPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameManager.instance.player.transform;
        colorOrig = model.material.color;

        GameManager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent != null ? agent.stoppingDistance : 0f;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(agent != null && agent.enabled && agent.isOnNavMesh && agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

    }

    protected bool CanSeePlayer()
    {
        if (player == null) return false;

        playerDir = player.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        if(angleToPlayer > FOV)
        {
            ResetStoppingDistanceToZero();
            return false;
        } 

        if(Physics.Raycast(transform.position, playerDir.normalized, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (agent) agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        ResetStoppingDistanceToZero();
        return false;
    }

    public void CheckRoam()
    {
        if (agent == null) return;

        if(roamTimer >= roamPauseTimer && agent !=null && agent.enabled && agent.isOnNavMesh && agent.remainingDistance < 0.01f)
        {
            Roam();
        }
        

    }

    public void Roam()
    {
        if (agent == null) return;

        roamTimer = 0f;
        agent.stoppingDistance = 0f;

        Vector3 ranPos = Random.insideUnitSphere * roamDistance + startingPos;
        if(NavMesh.SamplePosition(ranPos, out NavMeshHit hit, roamDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

    }

    void ResetStoppingDistanceToZero()
    {
        if (agent) agent.stoppingDistance = 0f;
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
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(player.position);
            }
                StartCoroutine(flashRed());
        }

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);

            if (enableDrops)
            {
                DropLoot();
            }
            
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
            ResetStoppingDistanceToZero();
        }

    }

    private void DropLoot()
    {
        foreach(var item in drops)
        {
            if (item.prefab == null) continue;

            if (Random.value > item.chance) continue;

            int count = Random.Range(item.minCount, item.maxCount + 1);
            for(int i = 0; i < count; i++)
            {
                Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
                Instantiate(item.prefab, pos, Quaternion.identity);
            }

        }
    }
}
