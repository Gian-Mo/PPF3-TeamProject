using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] GameObject model;
    [SerializeField] Animator anim;
    [SerializeField] int animTranSpeed;

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
        playerVel = playerVel.normalized * speed * Time.deltaTime;
        if (move.action.IsPressed())
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,Quaternion.LookRotation(playerVel.normalized),10 * Time.deltaTime); 
        }

       controller.Move(playerVel);

        SetAnimLoco();


    }
    void SetAnimLoco()
    {
        float agentSpeedCur = playerVel.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", agentSpeedCur);

    }
    void Shoot()
    {
        //Ray cast from the player head (change to hand later) towards the mouse position, distance is set by the weapon scriptable object(later)



    }
}
