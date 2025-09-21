using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class theyCallMeTutorial : MonoBehaviour
{
    public TextMeshPro text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (text != null)
        {
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (text != null)
        {
            //text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
    }
}
