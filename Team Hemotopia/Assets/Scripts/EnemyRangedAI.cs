using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedAI : EnemyAI_Base
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootCooldown;
    [SerializeField] float shootRange;

    float shootTimer;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        shootTimer += Time.deltaTime;

        if (!playerInTrigger)
        {
            CheckRoam();
            return;

        }

        if (CanSeePlayer())
        {
            float d = Vector3.Distance(transform.position, player.position);

            if(d > shootRange)
            {
                if (agent)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }
            else
            {
                if (agent)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }

                playerDir = player.position - transform.position;
                FaceTarget();

                if(shootTimer >= shootCooldown)
                {
                    shootTimer = 0f;
                    shoot();
                }
            }
        }
        else
        {
            CheckRoam();
        }

        
       

    }

    void shoot()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
        

    }
}
