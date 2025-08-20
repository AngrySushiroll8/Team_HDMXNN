using UnityEngine;
using UnityEngine.AI;


public class EnemyKiteDashMovement : EnemyMovement_Base
{
    [SerializeField] float preferredRange;
    [SerializeField] float closeThreshold;
    [SerializeField] float normalSpeed;

    [SerializeField] float dashDistance;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;

    float cooldownTimer;

    bool dashing;

    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        if (agent == null || !agent.enabled) return;

        cooldownTimer -= Time.deltaTime;

        float d = Dist(self, player);

        //finish dash
        if (dashing) return;

        //Dash away if too close and not on cooldown
        if (d < closeThreshold && cooldownTimer <= 0f)
        {
            Vector3 away = DirTo(player, self);
            Vector3 dashTarget = self.position + away * dashDistance;

            if(SampleOnNavMesh(dashTarget, dashDistance + 2f, out var navTarget))
            {
                cooldownTimer = dashCooldown;
                self.GetComponent<MonoBehaviour>().StartCoroutine(Dash(agent, navTarget));
                return;
            }
        }

        //Maintain position
        agent.speed = normalSpeed;
        agent.isStopped = false;

        if(d > preferredRange * 1.05f)
        {
            agent.SetDestination(player.position);
        }
        else if(d < preferredRange * 0.95f)
        {
            Vector3 away = DirTo(player, self);
            Vector3 stepBack = self.position + away * 2f;
            if(SampleOnNavMesh(stepBack, 2.5f, out var back))
                agent.SetDestination(back);
        }
        else
        {
            agent.ResetPath();
        }


    }

    System.Collections.IEnumerator Dash(NavMeshAgent agent, Vector3 target)
    {
        dashing = true;
        float prevSpeed = agent.speed;
        agent.speed = dashSpeed;
        agent.isStopped = false;
        agent.SetDestination(target);

        float t = 0f;
        while(t < dashDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        agent.speed = prevSpeed;
        dashing = false;
    }

    static bool SampleOnNavMesh(Vector3 pos, float maxDist, out Vector3 result)
    {
        if(NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDist, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = pos;
        return false;

    }

}
