
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ButtonController : MonoBehaviour
{
    public GameObject[] buttons;
    [SerializeField] InputActionReference up;
    [SerializeField] InputActionReference down;
    [SerializeField] InputActionReference enter;

    int currentSelected = 0;
    bool wasPressed;
    // Update is called once per frame
    void Update()
    {
        if (buttons.Length == 0)
        {
            wasPressed = false;
            buttons = GameObject.FindGameObjectsWithTag("Button");
        }
       
       if(wasPressed && buttons.Length > 0) buttons[currentSelected].GetComponent<Button>().Select();
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
       wasPressed = true;
   }
   void Down(InputAction.CallbackContext context)
   {
       if (currentSelected < buttons.Length - 1)
       {
           currentSelected++;
       }
       wasPressed = true;
   }
   void Enter(InputAction.CallbackContext context)
   {
       buttons[currentSelected].GetComponent<Button>().onClick.Invoke();
        wasPressed = true;
   }
}
