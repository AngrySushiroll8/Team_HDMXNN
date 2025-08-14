using UnityEngine;

public class LightEvent : MonoBehaviour
{
    Light lightController;
    [SerializeField] GameObject lightObject;
    Material lightMaterial;

    [Range(1, 10)]
    public bool isRangeChanging = false;
    public bool isIntensityChanging = false;
    public bool isColorChanging = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightController = GetComponent<Light>();
        lightMaterial = lightObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchLights();
    }
    void SwitchLights()
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
                lightMaterial.SetColor("_EmissionColor", Color.white);
            }
        }
    }
}
