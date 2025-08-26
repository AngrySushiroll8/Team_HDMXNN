using System.Threading;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    [SerializeField] Transform newPos;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("test1");
        if (other.transform.CompareTag("Player") && GameManager.instance.player.GetComponent<PlayerController>().teleporterCooldown >= 1.0f)
        {
            GameManager.instance.player.GetComponent<PlayerController>().teleporterCooldown = 0;
            GameManager.instance.player.transform.position = newPos.position;
            Debug.Log("test2");
        }
    }
}
