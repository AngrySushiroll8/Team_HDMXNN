using UnityEngine;
using System.Collections;

public class PlatformTriggerEvents : MonoBehaviour
{
    [SerializeField] Transform movingPlatform, destinationPoint;
    [SerializeField] float speed, pauseOnPosition;
    private Vector3 startPosition, endPosition;
    private bool isPlatformMoving = true, isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = movingPlatform.position;
        endPosition = destinationPoint.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovingPlatform();
    }
    public void MovingPlatform()
    {
        if (isPaused) return;
        Vector3 targetLocation = isPlatformMoving ? endPosition : startPosition;
        movingPlatform.position = Vector3.MoveTowards(movingPlatform.position, targetLocation, speed * Time.deltaTime);
        if (Vector3.Distance(movingPlatform.position, targetLocation) < 0.01f) StartCoroutine(PauseAndReverse());
    }
    private IEnumerator PauseAndReverse()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseOnPosition);
        isPlatformMoving = !isPlatformMoving;
        isPaused = false;
    }
}
