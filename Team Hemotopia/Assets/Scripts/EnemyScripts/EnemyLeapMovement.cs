
using UnityEngine;
using UnityEngine.AI;

public class EnemyLeapMovement : EnemyMovement_Base
{
    [SerializeField] float chaseSpeed;

    [SerializeField] float minLeapDist;
    [SerializeField] float maxLeapDist;
    [SerializeField] float leapForce;
    [SerializeField] float upwardBoost;
    [SerializeField] float leapCooldown;
    [SerializeField] float reenableAgentDelay;

    Rigidbody rb;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
    }

    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        timer += Time.deltaTime;

        float d = Dist(self, player);

        //defualt chasing
        if(d > maxLeapDist || d< minLeapDist || timer < leapCooldown)
        {
            if(agent != null && agent.enabled)
            {
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            return;
        }

        //Perform leap
        timer = 0f;

        if(agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        self.LookAt(player);

        Vector3 dir = DirTo(self, player);
        dir.y = upwardBoost;
        rb.AddForce(dir.normalized * leapForce, ForceMode.Impulse);

        self.GetComponent<MonoBehaviour>().StartCoroutine(ReenableAgent(agent, reenableAgentDelay));
        
    }

    System.Collections.IEnumerator ReenableAgent(NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null) agent.enabled = true;
    }



}
