using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        Moving,
        Stationary,
        DamageOverTime,
        Homing
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float damageRate;

    bool isDamaging;

    void Start()
    {
        if (type == DamageType.Moving || type == DamageType.Homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == DamageType.Moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    void Update()
    {
        if (type == DamageType.Homing)
        {
            rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type != DamageType.DamageOverTime)
        {
            dmg.TakeDamage(damageAmount);
        }

        if (type == DamageType.Moving || type == DamageType.Homing)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == DamageType.DamageOverTime)
        {
            if (!isDamaging)
            {
                StartCoroutine(DamageOther(dmg));
            }
        }
    }

    IEnumerator DamageOther(IDamage dmg)
    {
        isDamaging = true;
        dmg.TakeDamage(damageAmount);

        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }
}
