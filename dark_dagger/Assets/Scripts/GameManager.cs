using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text AmmoCurWeapon;
    [SerializeField] TMP_Text AmmoCurInventory;   

    public GameObject player;
    public playerController playerScript;

    public Image playerHP;
    public GameObject playerGetsDamaged;

    public InputActionReference menu;

    public bool isPaused = false;
    float timesScaleOrig;
     int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timesScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
       
    }    
 

    public void statePause()
    {
        isPaused = !isPaused;


        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;

        Time.timeScale = timesScaleOrig;       
       
        menuActive.SetActive(false);
        menuActive = null;
    }


    public void YouLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    void Pause (InputAction.CallbackContext context)
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
        }

    }


    private void OnEnable()
    {
        menu.action.started += Pause; 
    }
    private void OnDisable()
    {
        menu.action.started -= Pause;
    }

}
