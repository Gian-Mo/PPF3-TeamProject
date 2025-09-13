using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    public GameObject player;
    int hunt = 0;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 spawnPos = transform.position;
            player.transform.position = new Vector3(spawnPos.x, spawnPos.y + 1.0f, spawnPos.z);
           
        }
    }
    private void Update()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (player != null && hunt == 0)
        {
            Vector3 spawnPos = transform.position;
            player.transform.position = new Vector3(spawnPos.x, spawnPos.y + 1.0f, spawnPos.z);
            hunt++;
        }
    }
}
