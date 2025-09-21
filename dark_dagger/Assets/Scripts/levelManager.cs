using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;



public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    float timesScaleOrig;
    int gameGoalCount;

    [SerializeField] bool creation;
    public mapManager mapManagerScript;

    [SerializeField] private NavMeshSurface navMesh;

    public int level = 0;
    [SerializeField] int startSize = 3;

    public enemySpawn enemySpawner;
    public int currEnemy = 0;
    public bool exists = false;
    [SerializeField] private int bossAt = 3;
    public bool bossCurr = false;
    private Coroutine mapGenCoroutine;

    [SerializeField] GameManager gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timesScaleOrig = Time.timeScale;
        if (creation)
        {
            mapManagerScript = GetComponent<mapManager>();
            mapManagerScript.gridSize = startSize;
            StartCoroutine(genMap());
        }
    }

    private IEnumerator loadGenMap()
    {
        gameManager.ShowLoading(true);
        enemySpawner.resetEnemies();

        float waitTime = 3f;
        float timeDone = 0f;

        while (timeDone < waitTime)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                break;
            }
            timeDone += Time.deltaTime;
            yield return null;
        }


        if (level == bossAt)
        {
            bossCurr = true;
            mapManagerScript.bossGen();
        }
        else
        {
            bossCurr = false;
            mapManagerScript.generateMap();
        }

        if (navMesh != null)
            navMesh.BuildNavMesh();

        if (!bossCurr && enemySpawner != null)
        {
            List<Vector3> spawns = new List<Vector3>(mapManagerScript.enemySpawns);
            if (spawns.Count > 0)
            {
                for (int i = 0; i < 5 * (level + 1); i++)
                {
                    int ind = Random.Range(0, spawns.Count);
                    Vector3 pos = spawns[ind];
                    enemySpawner.spawnWithoutDoor(pos);
                    currEnemy++;
                }
            }

            enemySpawner.findSpawns();
        }
        exists = true;
        gameManager.ShowLoading(false);
    }

    private IEnumerator genMap()
    {
        yield return StartCoroutine(loadGenMap());
    }

    public void levelGen()
    {
        mapManagerScript.gridSize = startSize + level;

        if (mapGenCoroutine != null)
            StopCoroutine(mapGenCoroutine);

        mapGenCoroutine = StartCoroutine(loadGenMap());
    }

    void Update()
    {
        if (!bossCurr && enemySpawner != null)
        {
            if (currEnemy < ((level + 1) * 3) && exists)
            {
                enemySpawner.spawnRandomEnemy();
                currEnemy++;
            }
        }

        if (bossCurr)
        {
            GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
            if (bosses.Length == 0)
            {
                bossCurr = false;
                gameManager.YouWin();
            }
        }
    }

}