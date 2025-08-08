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
    void Update()
    {
        if (!playerInTrigger) return;

        shootTimer += Time.deltaTime;
        playerDir = player.position - transform.position;
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance > shootRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
            FaceTarget();

            if(shootTimer >= shootCooldown)
            {
                shootTimer = 0;
                shoot();
            }
        }
        
    }

    void shoot()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);

    }
}
