
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
        EnableInput();
    }
    void EnableInput()
    {
        up.action.started += (InputAction.CallbackContext context) =>
        {
            currentSelected--;
            wasPressed = true;
        };
        down.action.started += (InputAction.CallbackContext context) =>
        {
            currentSelected++;
            wasPressed = true;
        };
        enter.action.started += (InputAction.CallbackContext context) =>
        {
           buttons[currentSelected].GetComponent<Button>().onClick.Invoke();
            wasPressed = true;
        };
    }
}
