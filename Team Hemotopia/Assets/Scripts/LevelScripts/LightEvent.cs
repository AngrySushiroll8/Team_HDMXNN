using UnityEngine;

public class LightEvent : MonoBehaviour
{
    Light lightController;
    [SerializeField] GameObject lightObject;
    Material lightMaterial;

    [Range(1f,50f)][SerializeField] float lightRange;
    [Range(0f,1f)][SerializeField] float lightIntensity;
    [Range(0f,120f)][SerializeField] float lightAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LightSettings();
    }

    // Update is called once per frame
    void Update()
    {
        Flashlight();
    }
    void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lightController.isActiveAndEnabled)
            {
                lightController.enabled = false;
                lightMaterial.SetColor("_EmissionColor", Color.black);
            }
            else
            {
                lightController.enabled = true;
                lightMaterial.SetColor("_EmissionColor", Color.grey);
            }
        }
    }
    void LightSettings()
    {
        lightController = GetComponent<Light>();
        lightMaterial = lightObject.GetComponent<Renderer>().material;
        lightController.range = lightRange;
        lightController.intensity = lightIntensity;
        lightController.spotAngle = lightAngle;
    }
    
}
