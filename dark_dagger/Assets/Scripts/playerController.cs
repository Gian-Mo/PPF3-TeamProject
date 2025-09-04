using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] GameObject model;


   public Vector3 playerVel;

    public InputActionReference move;
    public InputActionReference shoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    void Move()
    {

            playerVel = move.action.ReadValue<Vector3>();
            playerVel *= speed * Time.deltaTime;
        if (move.action.IsPressed())
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,Quaternion.LookRotation(playerVel.normalized),10 * Time.deltaTime); 
        }

       controller.Move(playerVel);
        
        

    }

    void Shoot()
    {

    }
}
