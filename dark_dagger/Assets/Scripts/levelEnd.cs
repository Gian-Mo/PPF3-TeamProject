using UnityEngine;

public class levelEnd : MonoBehaviour
{
    public LevelManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     if(manager == null)
            manager = LevelManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(manager != null)
            {
                manager.level++;
                manager.exists = false;
                manager.levelGen();
            }
        }
    }
}
