using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] public gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null)
        {
            pickupable.getGunStats(gun);
            gun.ammoCur = gun.ammoMax;
            Destroy(gameObject);
        }
    }
}