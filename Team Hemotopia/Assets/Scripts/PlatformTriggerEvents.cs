using UnityEngine;
using System.Collections;

public class PlatformTriggerEvents : MonoBehaviour
{
    public Transform movingPlatform, position1, position2;
    public Vector3 newPosition;
    public string currentPosition;
    public float speed, resetTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movingPlatform.position = Vector3.Lerp(movingPlatform.position, newPosition, speed * Time.deltaTime);
    }
    void ChangeTarget()
    {
        if (currentPosition == "Moving To Position 1") {
            currentPosition = "Moving To Position 2";
            newPosition = position2.position;
        }
        else if (currentPosition == "Moving To Position 2") {
            currentPosition = "Moving To Position 1";
            newPosition = position1.position;
        }
        else {
            currentPosition = "Moving To Position 2";
            newPosition = position2.position;
        }
        Invoke("ChangeTarget", resetTime);
    }
}
