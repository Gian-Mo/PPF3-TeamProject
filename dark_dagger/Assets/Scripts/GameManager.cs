using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuLoading;

    [SerializeField] TMP_Text AmmoCurWeapon;
    [SerializeField] TMP_Text AmmoCurInventory;

    [SerializeField] PlayerInput PlayerInput;
    [SerializeField] ButtonController buttonController;

    public GameObject player;
    public playerController playerScript;

    public Image playerHP;
    public GameObject playerGetsDamaged;
   

    public InputActionReference menu;
    public InputActionReference menu2;

    public bool isPaused = false;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        timesScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        if (creation)
        {
            mapManagerScript = GetComponent<mapManager>();
            mapManagerScript.gridSize = startSize;
            StartCoroutine(genMap());
        }
    }


    public void statePause()
    {

        isPaused = !isPaused;


        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerInput.SwitchCurrentActionMap("Menus");
    }

    public void stateUnpause()
    {

        isPaused = !isPaused;

        Time.timeScale = timesScaleOrig;

        menuActive.SetActive(false);
        menuActive = null;
        PlayerInput.SwitchCurrentActionMap("Gameplay");
    }


    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
        StartCoroutine(FadeMenus(1f, 0.5f, menuActive));
    }
    void Pause(InputAction.CallbackContext context)
    {
        if (menuActive == null)
        {

            statePause();
            menuActive = menuPause;
            menuActive.SetActive(true);



        }
        else if (menuActive == menuPause)
        {
            stateUnpause();
          
           buttonController.buttons.Clear();
        }

    }

    IEnumerator FadeMenus(float to, float duration, GameObject objectToFade)
    {
        CanvasGroup canvasGroup = objectToFade.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, to, timer / duration);
            yield return null;
        }
    }
    private void OnEnable()
    {
        menu.action.started += Pause;
        menu2.action.started += Pause;

    }
    private void OnDisable()
    {
        menu.action.started -= Pause;
        menu2.action.started -= Pause;

    }

    private IEnumerator loadGenMap()
    {
        menuLoading.SetActive(true);
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
        mapManagerScript.incrementMapSize();
        mapManagerScript.generateMap();

        if (navMesh != null)
            navMesh.BuildNavMesh();

        if (enemySpawner != null)
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
        menuLoading.SetActive(false);
    }

    private IEnumerator genMap()
    {
        yield return StartCoroutine(loadGenMap());
    }

    public void levelGen()
    {
        mapManagerScript.gridSize = startSize + level;
        StartCoroutine(loadGenMap());
    }

    void Update()
    {
        if (enemySpawner != null)
        {
            currEnemy = enemySpawner.livingEnemies.Count;
            if (currEnemy < ((level + 1) * 3) && exists)
            {
                enemySpawner.spawnRandomEnemy();
                currEnemy++;
            }
        }
    }
}