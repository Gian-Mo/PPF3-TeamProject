using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] float increasePerSec;
    [SerializeField] float expandDuration;
    [SerializeField] float detectionDuration;
    IRadar radar;
    float timer;    
   
    void Update()
    {
        timer += Time.deltaTime;
        if (timer <= expandDuration) {
            transform.localScale += new Vector3(increasePerSec,0, increasePerSec) * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) return;

        radar = other.GetComponent<IRadar>();
        if (radar != null) {
            radar.getDetected(detectionDuration);           
        }
    }
    private void OnTriggerStay(Collider other)
    {
        radar = other.GetComponent<IRadar>();
        if (radar != null)
        {
            radar.setTimer(0f);
        }
      
    }
    
}
