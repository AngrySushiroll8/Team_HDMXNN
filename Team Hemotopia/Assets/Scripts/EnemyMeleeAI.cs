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
    void Update()
    {
        if (!playerInTrigger) return;

        attackTimer += Time.deltaTime;
        playerDir = player.position - transform.position;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if(distance <= attackRange)
        {
            agent.SetDestination(transform.position);
            FaceTarget();

            if(attackTimer >= attackCooldown)
            {
                attackTimer = 0;
                DoMeleeAttack();
            }
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
