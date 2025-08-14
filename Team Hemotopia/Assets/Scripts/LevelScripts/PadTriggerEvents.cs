using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class PadTriggerEvents : MonoBehaviour
{
    [SerializeField] int jumpHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject bounce = collision.gameObject;
        CharacterController character = bounce.GetComponent<CharacterController>();
        
    }
}
