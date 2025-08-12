using UnityEngine;

public class EnemyMeleeAI : EnemyAI_Base
{

    [SerializeField] GameObject weapon;
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] int meleeDamage;

    float attackTimer;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        attackTimer += Time.deltaTime;

        if (!playerInTrigger)
        {
            CheckRoam();
            return;
        }

        if (CanSeePlayer())
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if(distance > attackRange)
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

                if(attackTimer >= attackCooldown)
                {
                    attackTimer = 0f;
                    DoMeleeAttack();
                }
            }

        }
        else
        {
            CheckRoam();
        }
  
        
    }

    void DoMeleeAttack()
    {
        Debug.Log("Melee Attack");
        IDamage dmg = player.GetComponent<IDamage>();
        if (dmg != null )
        {
            dmg.TakeDamage(meleeDamage);
        }
    }
}
