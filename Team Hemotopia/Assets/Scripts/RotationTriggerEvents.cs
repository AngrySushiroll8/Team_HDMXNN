using UnityEngine;

public class RotationTriggerEvents : MonoBehaviour
{
    [SerializeField] float xRot, yRot, zRot;
    private Transform rotationTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotationTarget = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
    }
    void Rotation()
    {
        rotationTarget.Rotate(xRot * Time.deltaTime, yRot * Time.deltaTime, zRot * Time.deltaTime);

    }
}
