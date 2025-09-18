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

    public TMP_Text AmmoCurWeapon;
    public TMP_Text AmmoCurInventory;

    public PlayerInput PlayerInput;
    [SerializeField] ButtonController buttonController;

    public GameObject player;
    public playerController playerScript;

    public Image playerHP; 
    public Image playerAmmo;
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
    [SerializeField] private int bossAt = 3;
    public bool bossCurr = false;
    private Coroutine mapGenCoroutine;


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
        CursorManager.instance.SetMenusCursor();

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
       playerGetsDamaged.SetActive(false);
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
        StartCoroutine(FadeMenus(1f, 0.3f, menuActive));
    }
    public void YouWin()
    {
        playerGetsDamaged.SetActive(false);
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
        StartCoroutine(FadeMenus(1f, 0.3f, menuActive));
    }
    void Pause(InputAction.CallbackContext context)
    {
        if (menuActive == null)
        {

            playerScript.canChangeCursor = false;
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(true);



        }
        else if (menuActive == menuPause)
        {
            stateUnpause();

            CursorManager.instance.SetAimCursor();
            playerScript.canChangeCursor = true;

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


        if(level == bossAt)
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
        menuLoading.SetActive(false);
    }

    private IEnumerator genMap()
    {
        yield return StartCoroutine(loadGenMap());
    }

    public void levelGen()
    {
        mapManagerScript.gridSize = startSize + level;

        if(mapGenCoroutine != null)
            StopCoroutine(mapGenCoroutine);

        mapGenCoroutine = StartCoroutine(loadGenMap());
    }

    void Update()
    {
        if (!bossCurr && enemySpawner != null)
        {
            currEnemy = enemySpawner.livingEnemies.Count;
            if (currEnemy < ((level + 1) * 3) && exists)
            {
                enemySpawner.spawnRandomEnemy();
                currEnemy++;
            }
        }

        if (bossCurr)
        {
            GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
            if(bosses.Length == 0)
            {
                bossCurr = false;
                YouWin();
            }
        }
    }

    public void FlashScreen(Color color)
    {
        color.a = 0.15f;
        playerGetsDamaged.GetComponent<Image>().color = color;


        StartCoroutine(flashDamageScreen());
    }

    IEnumerator flashDamageScreen()
    {
        playerGetsDamaged.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerGetsDamaged.SetActive(false);
    }
}