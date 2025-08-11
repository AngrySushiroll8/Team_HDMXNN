using UnityEngine;

public class PowerupController : MonoBehaviour
{
    // To add a powerup type, add it to the enum and add a case in OnTriggerEnter
    enum PowerupType
    {
        Health,
        SpeedBoost
    }

    [SerializeField] PowerupType powerupType;

    [SerializeField] int healAmount = 20;
    [SerializeField] float speedBoostAmount = 1.5f;  // speed is multiplied by this




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (powerupType)
        {
            case PowerupType.Health:
                {
                    GameManager.instance.player.GetComponent<PlayerController>().TakeDamage(-healAmount);  // heal amount is added to player's health


                    break;
                }
            case PowerupType.SpeedBoost:
                {
                    //GameManager.instance.player.GetComponent<PlayerController>().


                    break;
                }
        }

        Destroy(gameObject);  
        //Destroy(GetComponent<Collider>()); 
    }
}
