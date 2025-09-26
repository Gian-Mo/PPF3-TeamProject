
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


public class ButtonController : MonoBehaviour
{
    public static ButtonController instance;
    public List<GameObject> buttons;
    public AudioResource[] sounds;
    public AudioSource source;
    [SerializeField] InputActionReference up;
    [SerializeField] InputActionReference down;
    [SerializeField] InputActionReference enter;

    int currentSelected = 0;
    bool wasPressed;
    // Update is called once per frame
    void Update()
    {
        if (instance == null)
        {
            instance = this; 
        }
        if (buttons.Count == 0)
        {
            wasPressed = false;
            buttons = new List<GameObject>(GameObject.FindGameObjectsWithTag("Button"));
            buttons.Sort(ButtonSort);
        }
       
       if(wasPressed && buttons.Count > 0 && currentSelected >= 0 && currentSelected < buttons.Count) buttons[currentSelected].GetComponent<Button>().Select();
    }

    private void OnEnable()
    {
        EnableInput(true);
    }
    private void OnDisable()
    {
        EnableInput(false);
    }
    void EnableInput(bool enable)
    {
        if (enable)
        {
            up.action.started += Up;
            down.action.started += Down;
            enter.action.started += Enter;

        }
        else
        {
            up.action.started -= Up;
            down.action.started -= Down;
            enter.action.started -= Enter;
        }
    }
   void Up(InputAction.CallbackContext context)
   {
       if (currentSelected > 0)
       {
           currentSelected--;

       }
        source.resource = sounds[0];
        source.Play();
       wasPressed = true;
   }
   void Down(InputAction.CallbackContext context)
   {
       if (currentSelected < buttons.Count - 1)
       {
           currentSelected++;

       }
        source.resource = sounds[0];
        source.Play();
        wasPressed = true;
   }
   void Enter(InputAction.CallbackContext context)
   {
        source.resource = sounds[1];
        source.Play();
        if (currentSelected != -1)
        {
            buttons[currentSelected].GetComponent<Button>().onClick.Invoke(); 
        }

        wasPressed = true;
   }

   
    public void ButtonClear()
    {
        currentSelected = -1;
        buttons.Clear();
    }

    int ButtonSort(GameObject a, GameObject b)
    {
        if (a.transform.position.y < b.transform.position.y)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
