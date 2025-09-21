using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Testest");
        }
    }
}
