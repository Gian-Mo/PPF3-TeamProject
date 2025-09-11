using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    private Transform[] spawnPoints;

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
    public void SpawnRandomEnemy()
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

        Instantiate(enemy, spawn.position, spawn.rotation);
    }
}
