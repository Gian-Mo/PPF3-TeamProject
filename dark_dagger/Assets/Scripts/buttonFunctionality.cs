using UnityEngine;
using UnityEngine.SceneManagement;


public class buttonFunctionality : MonoBehaviour
{
   public void Resume()
   {
        GameManager.instance.stateUnpause();
        GameManager.instance.playerScript.canChangeCursor = true;
    }
    public void Restart() {

        GameManager.instance.stateUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Settings()
    {
        GameManager.instance.OnSettings();
    }
    public void MainMenu()
    {
        GameManager.instance.stateUnpause();
        SceneManager.LoadScene("MainMenu");
    }
  
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;


#else

         Application.Quit(); Application.Quit();


#endif

    }
}
