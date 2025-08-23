using UnityEngine;

public class PowerupController : MonoBehaviour
{
    // To add a powerup type, add it to the enum and add a case in OnTriggerEnter
    enum PowerupType
    {
        Health,
        SpeedBoost,
        Doublejump
    }

    [SerializeField] PowerupType powerupType;

    [SerializeField] int healAmount = 20;
    [SerializeField] float speedBoostMultiplier = 1.5f;  // speed is multiplied by this
    [SerializeField] float effectDuration = 5f;




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
                    GameManager.instance.player.GetComponent<PlayerController>().HealPlayer(healAmount);
                    break;
                }
            case PowerupType.SpeedBoost:
                {
                    GameManager.instance.player.GetComponent<PlayerController>().SpeedBoost(speedBoostMultiplier, effectDuration);
                    break;
                }
            case PowerupType.Doublejump:
                {
                    GameManager.instance.player.GetComponent<PlayerController>().DoubleJump(effectDuration);
                    break;
                }
        }

        Destroy(gameObject);
    }
}
