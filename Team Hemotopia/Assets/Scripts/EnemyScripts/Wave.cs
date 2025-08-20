using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Tooltip("Must be the same length as the amounts.")]
    [SerializeField] GameObject[] enemyPrefabs;
    [Tooltip("Must be the same length as the prefabs.")]
    [SerializeField] int[] enemyAmounts;
    [SerializeField] Transform[] spawnPositions;

    Dictionary<GameObject, int> waveEnemies = new Dictionary<GameObject, int>();

    void Start()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            waveEnemies.Add(enemyPrefabs[i], enemyAmounts[i]);
        }
    }

    public void StartWave()
    {
        foreach (KeyValuePair<GameObject, int> pair in waveEnemies)
        {
            for (int enemyIndex = 0; enemyIndex < pair.Value; enemyIndex++)
            {
                Instantiate(pair.Key, spawnPositions.Length > 0 ? spawnPositions[Random.Range(0, spawnPositions.Length - 1)].position : transform.position, Quaternion.identity, null);
            }
        }
    }
}
