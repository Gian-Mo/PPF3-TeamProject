
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour, IDamage, IPickUp
{
    [SerializeField] CharacterController controller;
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] GameObject model;
    [SerializeField] Animator anim;
    [SerializeField] int animTranSpeed;
    [SerializeField] LineRenderer shootLine;
    [SerializeField] int shootDist;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform shootPos;
    [SerializeField] int meeleDamage;
    [SerializeField] float shootCoolDown;
    [SerializeField] float meeleCoolDown;


    int HPOrig;
    public Vector3 playerVel;

    public InputActionReference move;
    public InputActionReference shoot;
    public InputActionReference meele;
   public InputActionReference crouch;
    Vector3 mouseDirection;

    bool shootRot;
    bool ableToShoot;
    int ammoCur;
    int ammoMagMax;
    int totalAmmo;
    float meeleTimer;
    float shootTimer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    void Move()
    {
        meeleTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        playerVel = move.action.ReadValue<Vector3>();
        playerVel = playerVel.normalized * speed * Time.deltaTime;
        if (move.action.IsPressed())
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,Quaternion.LookRotation(playerVel.normalized),8 * Time.deltaTime); 
        }
        controller.Move(playerVel);

        if (shootRot)
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, Quaternion.LookRotation(new Vector3(mouseDirection.x, 0, mouseDirection.z)), 100 * Time.deltaTime); 
        }

        SetAnimLoco();

        if (ableToShoot) { 
            
            ShootBullet();
        
        }
    }
    void SetAnimLoco()
    {
        float playerSpeedCur = playerVel.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed",Mathf.Lerp( animSpeedCur, playerSpeedCur, Time.deltaTime * animTranSpeed));

    }
    void ShootBullet()
    {
        //Ray cast from the player head (change to hand later) towards the mouse position, distance is set by the weapon scriptable object(later)

        if (!GameManager.instance.isPaused)
        {
            if (shootTimer >= shootCoolDown)
            {   
                RaycastHit hit;
                mouseDirection = MousePos() - transform.position;
            

                Debug.DrawRay(transform.position, mouseDirection, Color.white, 0.5f);       
      

                if(Physics.Raycast(transform.position, mouseDirection.normalized, out hit, shootDist))
                {
                    // StartCoroutine(ShootFeedBack(hit));
                    if (Quaternion.Angle(Quaternion.LookRotation(new Vector3(mouseDirection.x, 0, mouseDirection.z)), model.transform.rotation) > 90)
                    {
                        StartCoroutine(TurnPlayerWhenShoot()); 
                    }
                    mouseDirection = new Vector3(mouseDirection.x, 0, mouseDirection.z);
                    Instantiate(projectile,shootPos.position,Quaternion.LookRotation(mouseDirection));
                }
       
                shootTimer = 0;
            }

        }
    }
    void MeeleAttack()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootPos.position,model.transform.forward,out hit,3))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(meeleDamage);

            }
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
        EnableShoot();
        meele.action.started += Meele;
    }

    private void OnDisable()
    {
       
        meele.action.started -= Meele;
    }

    private void EnableShoot()
    {
        shoot.action.performed += (InputAction.CallbackContext context) =>
        {
            ableToShoot = true;
        };
        shoot.action.canceled += (InputAction.CallbackContext context) =>
        {
            ableToShoot = false;
        };
    }
   
    private void Meele(InputAction.CallbackContext context) {

        if (!GameManager.instance.isPaused)
        {
            if (meeleTimer >= meeleCoolDown)
            {
                MeeleAttack(); 
                meeleTimer = 0;
            }
        }
    }


    IEnumerator ShootFeedBack(RaycastHit hit)
    {
        shootLine.enabled = true;
        shootLine.SetPosition(0, transform.position);
        shootLine.SetPosition(1, hit.point);

        yield return new WaitForSeconds(0.1f);

        shootLine.enabled = false;
    }
    IEnumerator TurnPlayerWhenShoot()
    {
        shootRot = true;
        yield return new WaitForSeconds(0.5f);
        shootRot = false;
    }

    public void takeDamage(int ammount)
    {
       HP -= ammount;
    }

    public void pickUp(int amount, int type)
    {
        if (type == 0) {

            if (HP + amount <= HPOrig) {

                HP += amount;
            }
            else
            {
                HP = HPOrig;
            }
        }
        if (type == 1) {

            totalAmmo += amount;
        }
    }
}
