using UnityEngine;

public class enemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform spawnPoint;

    void Start()
    {
        SpawnRandomEnemy();
    }

    void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemies assigned.");
            return;
        }
        int randInd = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = enemyPrefabs[randInd];

        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

}
