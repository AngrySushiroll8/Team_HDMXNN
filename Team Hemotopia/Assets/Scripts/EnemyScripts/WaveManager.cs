using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public int waveNumber = 1;
    public int numWaves;

    [SerializeField] GameObject wavesContainer;

    Wave[] waves;
    bool started = false;

    void Awake()
    {
        waveNumber = 1;
        numWaves = wavesContainer.transform.childCount;
        instance = this;
        waves = new Wave[numWaves];
        for (int childIndex = 0; childIndex < numWaves; childIndex++)
        {
            waves[childIndex] = wavesContainer.transform.GetChild(childIndex).GetComponent<Wave>();
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
        if (other.gameObject.CompareTag("Player") && waves.Length > 0 && !started)
        {
            StartWave(0);
            started = true;
        }
    }
}
