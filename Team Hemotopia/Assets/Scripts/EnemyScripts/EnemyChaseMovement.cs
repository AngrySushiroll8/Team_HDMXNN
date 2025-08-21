using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseMovement : EnemyMovement_Base
{
    [SerializeField] float chaseSpeed;
    [SerializeField] float viewDistance;
    [SerializeField] float fovAngle;
    [SerializeField] float eyeHeight;
    [SerializeField] float loseSightGrace;

    float lostTimer;

    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        if(agent == null || !agent.enabled || !agent.isOnNavMesh || player == null) return;

        Vector3 toPlayer = player.position - self.position;
        float dist = toPlayer.magnitude;
        if(dist > viewDistance)
        {
            lostTimer = Time.deltaTime;
            StopIfLost(agent);
            return;
            
        }

        Vector3 toPlayerFlat = toPlayer; toPlayer.y = 0f;
        Vector3 fwdFlat = self.forward; fwdFlat.y = 0f;
        if (toPlayerFlat.sqrMagnitude < 0.0001f) return;
        
        float angle = Vector3.Angle(fwdFlat.normalized, toPlayerFlat.normalized);
        if(angle > fovAngle)
        {
            lostTimer += Time.deltaTime;
            StopIfLost(agent);
            return;
        }

        Vector3 eyePos = self.position + Vector3.up * eyeHeight;
        Vector3 dir = (player.position - eyePos).normalized;
        if(!Physics.Raycast(eyePos, dir, out RaycastHit hit, viewDistance, ~0, QueryTriggerInteraction.Ignore) || !hit.collider.CompareTag("Player"))
        {
            lostTimer -= Time.deltaTime;
            StopIfLost(agent);
            return; 
        }

        lostTimer = 0f;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);

    }

    void StopIfLost(NavMeshAgent agent)
    {
        if(lostTimer >= loseSightGrace)
        {
            if(agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
    }
}
