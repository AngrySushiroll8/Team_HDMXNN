using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Tooltip("Must be the same length as the amounts.")]
    [SerializeField] GameObject[] enemyPrefabs;
    [Tooltip("Must be the same length as the prefabs.")]
    [SerializeField] int[] enemyAmounts;
    [SerializeField] int flyingEnemyHeight;

    Transform[] spawnPositions;
    Dictionary<GameObject, int> waveEnemies = new Dictionary<GameObject, int>();

    public GameObject positionContainer;
    List<GameObject> enemiesToSpawn = new List<GameObject>();
    float enemyTimer = 0;

    void Start()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            waveEnemies.Add(enemyPrefabs[i], enemyAmounts[i]);
        }

        spawnPositions = new Transform[positionContainer.transform.childCount];
        for (int posIndex = 0; posIndex < spawnPositions.Length; posIndex++)
        {
            spawnPositions[posIndex] = positionContainer.transform.GetChild(posIndex);
        }
    }

    void Update()
    {
        if (enemiesToSpawn.Count > 0)
        {
            enemyTimer += Time.deltaTime;
            if (enemyTimer > 0.15f)
            {
                int spawnIndex = Random.Range(0, spawnPositions.Length - 1);
                Instantiate(enemiesToSpawn[0], spawnPositions.Length > 0 ? (enemiesToSpawn[0].CompareTag("FlyingEnemy") ? spawnPositions[spawnIndex].position + (spawnPositions[spawnIndex].up * flyingEnemyHeight) : spawnPositions[spawnIndex].position) : transform.position, Quaternion.identity, null);
                enemiesToSpawn.RemoveAt(0);
                enemyTimer = 0;
                Debug.Log("Spawn");
            }
        }
    }

    public void StartWave()
    {
        foreach (KeyValuePair<GameObject, int> pair in waveEnemies)
        {
            for (int enemyIndex = 0; enemyIndex < pair.Value; enemyIndex++)
            {
                enemiesToSpawn.Add(pair.Key);
            }
        }
    }
}
