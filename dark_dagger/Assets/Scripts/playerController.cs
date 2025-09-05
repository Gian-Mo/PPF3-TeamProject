using Mono.Cecil;
using System;
using System.Collections;
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
    [SerializeField] LineRenderer shootLine;

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
        float playerSpeedCur = playerVel.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed",Mathf.Lerp( animSpeedCur, playerSpeedCur, Time.deltaTime * animTranSpeed));

    }
    void Shoot()
    {
        //Ray cast from the player head (change to hand later) towards the mouse position, distance is set by the weapon scriptable object(later)

        RaycastHit hit;
         Vector3 mouseDirection = MousePos() - transform.position;

        Debug.DrawRay(transform.position, mouseDirection, Color.white, 0.5f);
        
        if(Physics.Raycast(transform.position, mouseDirection.normalized, out hit))
        {
          StartCoroutine(ShootFeedBack(hit));
        }
       
    }
    Vector3 MousePos()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;

       Physics.Raycast(ray, out hit);       

        return hit.point;
    }

    private void OnEnable()
    {
       shoot.action.started += OnShoot;
    }
   private void OnDisable()
    {
       shoot.action.started -= OnShoot;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        Shoot();
        Debug.Log("Fired");
    }


    IEnumerator ShootFeedBack(RaycastHit hit)
    {
        shootLine.enabled = true;
        shootLine.SetPosition(0, transform.position);
        shootLine.SetPosition(1, hit.point);

        yield return new WaitForSeconds(0.1f);

        shootLine.enabled = false;
    }
}
