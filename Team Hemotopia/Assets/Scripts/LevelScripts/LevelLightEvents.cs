using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLightEvents : MonoBehaviour
{
    [SerializeField] Light lightObject;
    [SerializeField] Color color1, color2;
    [SerializeField] bool isRangeChanging, isIntensityChanging, isColorChanging, isRandChanging;
    [SerializeField] float lightRangeSpeed, lightIntensitySpeed;
    [SerializeField] float lightColorSpeed;
    float gameTimer, randInterval, randTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightObject = GetComponent<Light>();
        gameTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        randInterval += Time.deltaTime;
        if (isRandChanging && randTimer >= randInterval) {
            lightRangeSpeed = Random.Range(0f, 5f);
            lightIntensitySpeed = Random.Range(0f, 10f);
            lightColorSpeed = Random.Range(1f, 5f);
        }
        if (isRangeChanging) {
            lightObject.range = Mathf.PingPong(Time.time * lightRangeSpeed, lightRangeSpeed);
        }
        if (isIntensityChanging) {
            lightObject.intensity = Mathf.PingPong(Time.time * lightIntensitySpeed, lightIntensitySpeed);
        }
        if (isColorChanging) {
            float timer = (Mathf.Sin((Time.time - gameTimer) * lightColorSpeed) + 1f) * 0.5f;
            lightObject.color = Color.Lerp(color1, color2, timer);
        }
    }
}
