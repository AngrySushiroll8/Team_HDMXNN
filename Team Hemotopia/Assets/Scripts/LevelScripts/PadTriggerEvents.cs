using System;
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
    private void OnTriggerEnter(Collider collider)
    {
        //Extra teleport line for later
        //GameManager.instance.player.transform.position = new Vector3(currentPos.x, currentPos.y + jumpHeight, currentPos.z);
        
        // 50/50 Jump Pad
        GameManager.instance.player.GetComponent<PlayerController>().jumpVec.y = jumpHeight;
    }
}
