
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

   

    public gunStats currGun;
    [SerializeField] GameObject gunModel;

    int HPOrig;
    float heighOrig;
    public Vector3 playerVel;

    public InputActionReference move;
    public InputActionReference shoot;
    public InputActionReference meele;
   public InputActionReference crouch;
    Vector3 mouseDirection;

    bool shootRot;
    bool ableToShoot;
    bool ableToCrouch;
    bool health;
    int ammoCur;
    int ammoMagMax;
    int totalAmmo;
    float meeleTimer;
    float shootTimer;

    float gunNoiseLevel;
    SphereCollider objectCollider;
    public float noiseLevel = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        heighOrig = controller.height;
        anim.SetBool("Pistol", true);

        objectCollider = GetComponent<SphereCollider>();
        objectCollider.radius = noiseLevel;
    }

    // Update is called once per frame
    void Update()
    {
        objectCollider.radius = noiseLevel;
        Move();
    }


    void Move()
    {
        meeleTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        playerVel = move.action.ReadValue<Vector3>();
        playerVel = playerVel.normalized * speed * Time.deltaTime;
        noiseLevel = 0;
        if (move.action.IsPressed())
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,Quaternion.LookRotation(playerVel.normalized),8 * Time.deltaTime);
            noiseLevel = 2;
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

        Crouch();

       UpdatePalyerUI();
    }

    void Crouch()
    {
        anim.SetBool("Crouch",ableToCrouch);
        if (ableToCrouch)
        {
            controller.height = Mathf.Lerp(controller.height,1.2f,8 * Time.deltaTime);
            controller.center = Vector3.Lerp(controller.center, new Vector3(0, -0.3f, 0), 8 * Time.deltaTime);
            noiseLevel = 1;
        }
        else
        {
            if (controller.height != heighOrig && controller.center != new Vector3(0, 0, 0))
            {
                controller.height = Mathf.Lerp(controller.height, heighOrig, 8 * Time.deltaTime);
                controller.center = Vector3.Lerp(controller.center, new Vector3(0, 0, 0), 8 * Time.deltaTime);
            }
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
                    GameObject bullet = Instantiate(projectile,shootPos.position,Quaternion.LookRotation(mouseDirection));
                    Damage gunDmg = bullet.GetComponent<Damage>();
                    if(gunDmg != null && currGun != null)
                        gunDmg.setDamage(currGun.shootDamage);
                    noiseLevel += gunNoiseLevel;
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
        EnableShoot(true);
        EnableCrouch(true);
        meele.action.started += Meele;
    }

    private void OnDisable()
    {
        EnableShoot(false);
        EnableCrouch(false);
        meele.action.started -= Meele;
    }

    private void EnableShoot(bool enable)
    {
        if (enable)
        {
            shoot.action.started += ShootTrue;
            shoot.action.performed += ShootTrue;
            shoot.action.canceled += ShootFalse; 
        }
        else
        {
            shoot.action.started -= ShootTrue;
            shoot.action.performed -= ShootTrue;
            shoot.action.canceled -= ShootFalse;
        }
    }
    private void EnableCrouch(bool enable)
    {
        if (enable)
        {
            crouch.action.started += CrouchTrue;
            crouch.action.performed += CrouchTrue;
            crouch.action.canceled += CrouchFalse;
        }
        else
        {
            crouch.action.started -= CrouchTrue;
            crouch.action.performed -= CrouchTrue;
            crouch.action.canceled -= CrouchFalse;
        }
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
    void CrouchTrue(InputAction.CallbackContext context)
    {
        ableToCrouch = true;
    }
    void ShootTrue(InputAction.CallbackContext context)
    {
        ableToShoot = true;
    }
    void CrouchFalse(InputAction.CallbackContext context)
    {
        ableToCrouch = false;
    }
    void ShootFalse(InputAction.CallbackContext context)
    {
        ableToShoot = false;
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

    public void UpdatePalyerUI()
    {
        if (health)
        {
            GameManager.instance.playerHP.fillAmount = Mathf.Lerp(GameManager.instance.playerHP.fillAmount,(float)HP / HPOrig, 2 * Time.deltaTime); 
            if (GameManager.instance.playerHP.fillAmount == (float)HP / HPOrig)
            {
                health = false;
            }
        }
    }
    public void takeDamage(int ammount)
    {
       HP -= ammount;

        health = true; 
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

            health = true;
        }
        if (type == 1) {

            totalAmmo += amount;
        }
    }

    public void equipGun(gunStats gun)
    {
        currGun = gun;
        shootDist = gun.shootDistance;
        shootCoolDown = gun.shootRate;
        gunNoiseLevel = gun.shootVol * 10;
        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
