using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    public GameObject player;
    int grab = 0;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (player != null)
            grabPlayer();
    }
    private void Update()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (player != null && grab == 0)
        {
            grabPlayer();
            grab++;
        }
    }

    public void grabPlayer()
    {
        if (player != null)
        {
            player.GetComponent<playerController>().enabled = false;
            Vector3 spawnPos = transform.position;
            player.transform.position = new Vector3(spawnPos.x, spawnPos.y + 1.0f, spawnPos.z);
            player.GetComponent<playerController>().enabled = true;
        }
    }
}
