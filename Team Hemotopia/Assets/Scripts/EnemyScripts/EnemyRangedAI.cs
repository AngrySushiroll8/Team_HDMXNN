using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedAI : EnemyAI_Base
{

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootCooldown;
    [SerializeField] float shootRange;

    EnemyMovement_Base movement;

    float shootTimer;


    void Awake()
    {
        movement = GetComponent<EnemyMovement_Base>();
    }

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

            movement?.Move(agent, transform, player);

            var kite = movement as EnemyKiteDashMovement;

            if ((kite != null && (kite.IsDashing || d < kite.CloseThreshold)))
                return;


            if (d > shootRange) return;
            
            if (agent != null && agent.enabled && agent.isOnNavMesh)
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
        else
        {
            CheckRoam();
        }

        
       

    }

    void shoot()
    {

        if (!bullet || !shootPos) return;

        Vector3 dir = (player.position - shootPos.position).normalized;

        GameObject proj = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(dir));

    }
}
