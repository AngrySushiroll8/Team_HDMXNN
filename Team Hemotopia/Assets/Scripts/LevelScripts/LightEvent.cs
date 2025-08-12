using UnityEngine;

public class LightEvent : MonoBehaviour
{
    Light lightController;
    [SerializeField] GameObject lightObject;
    Material lightMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightController = GetComponent<Light>();
        lightMaterial = lightObject.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchLights();
        }
    }
    void SwitchLights()
    {
        if(lightController.isActiveAndEnabled)
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
