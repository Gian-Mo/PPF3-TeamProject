using UnityEngine;

public class theKey : MonoBehaviour
{
    private GameObject locked;

    void Start()
    {
        locked = GameObject.FindGameObjectWithTag("Locked");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(locked != null)
                Destroy(locked);
            Destroy(gameObject);
        }
    }
}
