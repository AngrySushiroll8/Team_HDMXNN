using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public int waveNumber = 1;

    Wave[] waves;

    void Awake()
    {
        waveNumber = 1;
        instance = this;
        waves = new Wave[transform.childCount];
        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            waves[childIndex] = transform.GetChild(childIndex).GetComponent<Wave>();
        }
    }

    public bool StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Length) return false;
        waves[waveIndex].StartWave();
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && waves.Length > 0)
        {
            StartWave(0);
        }
    }
}
