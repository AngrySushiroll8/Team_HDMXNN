using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{

    [SerializeField] GameObject weapon;
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] int meleeDamage;

    float attackTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MeleeAttack()
    {

    }
}
