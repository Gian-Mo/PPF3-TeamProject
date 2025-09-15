using UnityEngine;

public class victory : MonoBehaviour
{ 
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.YouWin(); 
        }
    }

}
