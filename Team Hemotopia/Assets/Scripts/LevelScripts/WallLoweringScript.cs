using UnityEngine;

public class WallLoweringScript : MonoBehaviour
{
    [SerializeField] int distance;
    [SerializeField] Vector3 newPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.player.GetComponent<PlayerController>().gunList.Count > 0)
        {
            if (Vector3.Distance(transform.localPosition, newPosition) > 0.1f)
            {
                Debug.Log("while loop running");
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x + distance, transform.localPosition.y, transform.localPosition.z), 1.0f * Time.deltaTime);
            }
        }
    }
}
