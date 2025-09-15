using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;

    public InputActionReference back;

    void Start()
    {
       

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Testest");
      
    }
    public void StartShowcase()
    {
        SceneManager.LoadScene("Showcase");

    }
    public void Settings()
   {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);

   }

   void Back(InputAction.CallbackContext context)
   {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);

   }
    

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;


#else

         Application.Quit(); Application.Quit();


#endif

    }

    private void OnEnable()
    {
        back.action.started += Back;
    }
    private void OnDisable()
    {
        back.action.started -= Back;
    }


}
