using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLightEvents : MonoBehaviour
{
    [SerializeField] Light lightObject;
    [SerializeField] Color color1, color2;
    [SerializeField] bool isRangeChanging, isIntensityChanging, isColorChanging;
    [Range(1f, 10f)][SerializeField] float lightRangeSpeed, lightIntensitySpeed;
    [SerializeField] float lightColorSpeed;
    float gameTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightObject = GetComponent<Light>();
        gameTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRangeChanging) {
            lightObject.range = Mathf.PingPong(Time.time * lightRangeSpeed, lightRangeSpeed);
        }
        if (isIntensityChanging) {
            lightObject.intensity = Mathf.PingPong(Time.time * lightIntensitySpeed, lightIntensitySpeed);
        }
        if (isColorChanging) {
            float timer = (Mathf.Sin(Time.time - gameTimer * lightColorSpeed));
            lightObject.color = Color.Lerp(color1, color2, timer);
        }
    }
}
