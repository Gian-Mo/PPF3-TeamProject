using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using System;
using UnityEditor;



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

    public GameObject player;
    public playerController playerScript;

    public Image playerHP;
    public GameObject playerGetsDamaged;

    public InputActionReference menu;
    public InputActionReference menu2;

    public bool isPaused = false;
    float timesScaleOrig;
    int gameGoalCount;


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

}
