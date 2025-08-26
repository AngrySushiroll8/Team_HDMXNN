using UnityEngine;
using UnityEngine.SceneManagement;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;
    Vector3 startingPos;
    Vector3 upPos;
    bool upDown;

    private void Start()
    {
        startingPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
    }
    private void Update()
    {
        
        transform.Rotate(0, 90 * Time.deltaTime, 0);
        floatingUpAndDown();
    }

    void floatingUpAndDown()
    {
        if (!upDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos, 0.3f * Time.deltaTime);
            if (Vector3.Distance(transform.position, upPos) < 0.01f) upDown = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPos, 0.3f * Time.deltaTime);
            if (Vector3.Distance(transform.position, startingPos) < 0.01f) upDown = false;
        }
    }

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