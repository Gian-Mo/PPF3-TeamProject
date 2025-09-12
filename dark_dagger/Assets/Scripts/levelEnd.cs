using UnityEngine;

public class levelEnd : MonoBehaviour
{
    public GameManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     if(manager == null)
            manager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(manager != null)
            {
                //manager.levelGen();
            }
        }
    }
}
