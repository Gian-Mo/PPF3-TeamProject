using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;
using System;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuSettings;
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
    public Image playerRadar;
    public GameObject playerGetsDamaged;


    public InputActionReference menu;
    public InputActionReference menu2;

    public bool isPaused = false;
    public float timesScaleOrig;

    [SerializeField] GameObject hotBar;


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
        updateHotBar();
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

    public void OnSettings()
    {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
        menu2.action.started -= Pause;
        menu2.action.started += Back;

    }

    private void Back(InputAction.CallbackContext context)
    {
        OffSettings();
    }

    public void OffSettings()
    {
        menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
        menu2.action.started -= Back;
        menu2.action.started += Pause;
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


    void Update()
    {

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

    public void ShowLoading(bool show)
    {
        menuLoading.SetActive(show);
    }

    public void updateHotBar()
    {
        Transform item1 = hotBar.transform.Find("Item1");
        Transform item2 = hotBar.transform.Find("Item2");
        resetHotBar(item1);
        resetHotBar(item2);
        if (playerScript.gunList.Count > 1)
        {
            string type = playerScript.gunList[0].type;
            switch (type)
            {
                case "Pistol":
                    item1.Find("Pistol").gameObject.SetActive(true);
                    break;
                case "Rifle":
                    item1.Find("Rifle").gameObject.SetActive(true);
                    break;
                case "Sniper":
                    item1.Find("Sniper").gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("NOT A GUN TYPE, OH GOD, HELP");
                    break;
            }
            type = playerScript.gunList[1].type;
            switch (type)
            {
                case "Pistol":
                    item2.Find("Pistol").gameObject.SetActive(true);
                    break;
                case "Rifle":
                    item2.Find("Rifle").gameObject.SetActive(true);
                    break;
                case "Sniper":
                    item2.Find("Sniper").gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("NOT A GUN TYPE, OH GOD, HELP");
                    break;
            }
            if(playerScript.gunListPos == 0)
            {
                item1.Find("BoxSelect").gameObject.SetActive(true);
                item1.Find("Box").gameObject.SetActive(false);
            }
            if (playerScript.gunListPos == 1)
            {
                item2.Find("BoxSelect").gameObject.SetActive(true);
                item2.Find("Box").gameObject.SetActive(false);
            }
        }
    }

    private void resetHotBar(Transform item)
    {
        item.Find("Pistol").gameObject.SetActive(false);
        item.Find("Rifle").gameObject.SetActive(false);
        item.Find("Sniper").gameObject.SetActive(false);
        item.Find("BoxSelect").gameObject.SetActive(false);
        item.Find("Box").gameObject.SetActive(true);
    }
}