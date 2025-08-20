
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

    public bool IsDashing => dashing;
    public float CloseThreshold => closeThreshold;

    float cooldownTimer;

    bool dashing;


    public override void Move(NavMeshAgent agent, Transform self, Transform player)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

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
                self.GetComponent<MonoBehaviour>().StartCoroutine(DashBack(agent, self, player, navTarget));
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
            Vector3 back = self.position + away * 2f;
            if(SampleOnNavMesh(back, 3f, out var stepBack))
                agent.SetDestination(stepBack);
        }
        else
        {
            agent.ResetPath();
        }


    }

    System.Collections.IEnumerator DashBack(NavMeshAgent agent,Transform self, Transform player, Vector3 target)
    {
        dashing = true;

        float prevSpeed = agent.speed;
        float prevAccel = agent.acceleration;
        bool prevStopped = agent.isStopped;
        bool prevUpdateRot = agent.updateRotation;
        bool prevAutoBraking = agent.autoBraking;

        agent.updateRotation = false;
        agent.autoBraking = false;
        agent.acceleration = 1000f;
        agent.speed = dashSpeed;
        agent.isStopped = false;
        agent.SetDestination(target);
       
        float t = 0f;
        while(t < dashDuration)
        {
            t += Time.deltaTime;

            Vector3 look = player.position - self.position; look.y = 0f;
            if(look.sqrMagnitude > 0.0001f)
            {
                Quaternion desired = Quaternion.LookRotation(look);
                self.rotation = Quaternion.Slerp(self.rotation, desired, Time.deltaTime * 12f);
            }

            yield return null;
            
        }

        agent.updateRotation = prevUpdateRot;
        agent.autoBraking = prevAutoBraking;
        agent.acceleration = prevAccel;
        agent.speed = prevSpeed;
        agent.isStopped = prevStopped;

        dashing = false;

        
    }


}
