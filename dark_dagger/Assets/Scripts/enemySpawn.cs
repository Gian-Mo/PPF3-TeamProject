using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    private Transform[] spawnPoints;
    private List<GameObject> livingEnemies = new List<GameObject>();

    public void findSpawns()
    {
        GameObject[] spawnPointsObj = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnPointsObj.Length == 0)
        {
            Debug.LogWarning("No spawn points found");
            spawnPoints = new Transform[0];
            return;
        }
        spawnPoints = new Transform[spawnPointsObj.Length];
        for (int i = 0; i < spawnPointsObj.Length; i++)
            spawnPoints[i] = spawnPointsObj[i].transform;
    }
    public void spawnRandomEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemies assigned.");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found");
            return;
        }

        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject enemy = enemyPrefabs[enemyIndex];
        Transform spawn = spawnPoints[spawnIndex];

        GameObject summoned = Instantiate(enemy, spawn.position, spawn.rotation);
        livingEnemies.Add(summoned);
    }

    public void spawnWithoutDoor(Vector3 pos)
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemies assigned.");
            return;
        }
        int enemyInd = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = enemyPrefabs[enemyInd];
        Vector3 spawnPos = pos + new Vector3(0, 0.5f, 0);
        GameObject summoned = Instantiate(enemy, spawnPos, Quaternion.identity);
        livingEnemies.Add(summoned);
    }

    public void resetEnemies()
    {
        foreach (GameObject alive in livingEnemies)
        {
            if (alive != null)
                Destroy(alive);
        }
        livingEnemies.Clear();
    }
}
