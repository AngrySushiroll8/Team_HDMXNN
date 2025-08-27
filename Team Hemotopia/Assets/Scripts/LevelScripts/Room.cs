using UnityEngine;

public class Room : MonoBehaviour
{
    public int waveNumber = 1;

    [SerializeField] GameObject wavesContainer;
    [SerializeField] GameObject posContainer;

    public Wave[] waves;
    public bool started = false;

    void Awake()
    {
        waveNumber = 1;
        waves = new Wave[wavesContainer.transform.childCount];
        for (int childIndex = 0; childIndex < wavesContainer.transform.childCount; childIndex++)
        {
            waves[childIndex] = wavesContainer.transform.GetChild(childIndex).GetComponent<Wave>();
            waves[childIndex].positionContainer = posContainer;
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
            GameManager.instance.wavePlusEnemyUI.SetActive(true);
            StartWave(0);
            GameManager.instance.waveCurrent.text = waveNumber.ToString("F0");
            GameManager.instance.waveTotal.text = waves.Length.ToString("F0");
            started = true;
        }
    }
}
