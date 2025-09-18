
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
    int speedOrig; 
    public Vector3 playerVel;

    public InputActionReference move;
    public InputActionReference shoot;
    public InputActionReference meele;
   public InputActionReference crouch; 
    public InputActionReference reload;
    Vector3 mouseDirection;

    bool shootRot;
    bool ableToShoot;
    bool ableToCrouch;
    bool inShootDist;
    bool healthUpdate;
    bool healing;
    bool ammoUpdate;
    public bool canChangeCursor;
    public int totalAmmo;
    float meeleTimer;
    float shootTimer;

    float gunNoiseLevel;
    SphereCollider objectCollider;
    public float noiseLevel = 0;
    GameObject actualGun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        HPOrig = HP;
        heighOrig = controller.height;
        speedOrig = speed;
        anim.SetBool("Pistol", true);
        canChangeCursor = true;

        objectCollider = GetComponent<SphereCollider>();
        objectCollider.radius = noiseLevel;
        equipGun(currGun);
        UpdateAmmo();
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

       

        SetAnimLoco();

        if (canChangeCursor)
        {
            inShootDist = CanShoot(); 

            if (inShootDist) {
                CursorManager.instance.SetAimCursor();
            }
            else
            {
                CursorManager.instance.SetFadedAimCursor();
            }
        }

        if (ableToShoot)
        {

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
    void SetShootAnim()
    {
        actualGun.GetComponent<Animator>().SetTrigger("Shoot");        

    }
    void MuzzleEffect()
    {
       
        Instantiate(currGun.muzzleFlash, gunModel.transform);
        
    }
   void ReloadWeapon() { 

        if(currGun.ammoMax - currGun.ammoCur >= totalAmmo )
        {
            currGun.ammoCur += totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            totalAmmo -= currGun.ammoMax - currGun.ammoCur;
            currGun.ammoCur = currGun.ammoMax;
        }
        ammoUpdate = true;
        UpdateAmmo();
   }

    bool CanShoot()
    {
        Vector3 mouseTemp = MousePos();
        if(Vector3.Distance(mouseTemp,transform.position) <= shootDist)
        {
            mouseDirection = mouseTemp - transform.position;
            return true;
        }  

        return false;
    }
    void ShootBullet()
    {
        //Ray cast from the player head (change to hand later) towards the mouse position, distance is set by the weapon scriptable object(later)

        if (!GameManager.instance.isPaused)
        {
            if (shootTimer >= shootCoolDown && currGun.ammoCur > 0)
            {                      

                if(inShootDist)
                {

                    SetShootAnim();
                   
                    StartCoroutine(TurnPlayerWhenShoot(0.025f)); 
                    
                    mouseDirection = new Vector3(mouseDirection.x, 0, mouseDirection.z);
                
                    
                    MuzzleEffect();
                    GameObject bullet = Instantiate(projectile, shootPos.position, Quaternion.LookRotation(mouseDirection));
                    Damage gunDmg = bullet.GetComponent<Damage>();
                    if (gunDmg != null && currGun != null)
                        gunDmg.setDamage(currGun.shootDamage);

                    noiseLevel = gunNoiseLevel;

                    currGun.ammoCur--;
                    ammoUpdate = true;

                    UpdateAmmo(); 
                  
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
             StartCoroutine(MeeleFeedBack(hit));
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
        reload.action.started += Reload;
    }

    private void OnDisable()
    {
        EnableShoot(false);
        EnableCrouch(false);
        meele.action.started -= Meele;
        reload.action.started -= Reload;
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
    private void Reload(InputAction.CallbackContext context) { 
    
        ReloadWeapon();
    }
    void CrouchTrue(InputAction.CallbackContext context)
    {
        ableToCrouch = true;
        speed = 3;
    }
    void ShootTrue(InputAction.CallbackContext context)
    {
        ableToShoot = true;
    }
    void CrouchFalse(InputAction.CallbackContext context)
    {
        ableToCrouch = false;
        speed = speedOrig;
    }
    void ShootFalse(InputAction.CallbackContext context)
    {
        ableToShoot = false;
    }


    IEnumerator MeeleFeedBack(RaycastHit hit)
    {
        shootLine.enabled = true;
        shootLine.SetPosition(0, transform.position);
        shootLine.SetPosition(1, hit.point);
        shootLine.startColor = Color.red;
        shootLine.endColor = Color.yellow;

        yield return new WaitForSeconds(0.1f);

        shootLine.enabled = false;
    }
    IEnumerator TurnPlayerWhenShoot(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, Quaternion.LookRotation(new Vector3(mouseDirection.x, 0, mouseDirection.z)),timer/duration); 
            yield return null;
        }
     
    }

    public void UpdatePalyerUI()
    {
        //HP
        if (healthUpdate)
        {
            GameManager.instance.playerHP.fillAmount = Mathf.Lerp(GameManager.instance.playerHP.fillAmount, (float)HP / HPOrig, 2 * Time.deltaTime);

            if (!healing)
            {
                if (GameManager.instance.playerHP.fillAmount - ((float)HP / HPOrig) < 0.01)
                {
                    healthUpdate = false;

                } 
            }
            else
            {

                if ( ((float)HP / HPOrig) - GameManager.instance.playerHP.fillAmount < 0.001)
                {
                    healthUpdate = false;
                    healing = false;

                }
            }
        }


        //Ammo
        if (ammoUpdate)
        {

            GameManager.instance.playerAmmo.fillAmount = Mathf.Lerp(GameManager.instance.playerAmmo.fillAmount, (float)currGun.ammoCur / currGun.ammoMax, 2 * Time.deltaTime);

           
        }

    }
    void UpdateAmmo()
    {
        GameManager.instance.AmmoCurWeapon.SetText(currGun.ammoCur.ToString());
        GameManager.instance.AmmoCurInventory.SetText(totalAmmo.ToString());
    }
    public void takeDamage(int ammount)
    {
       HP -= ammount;
       GameManager.instance.FlashScreen(Color.softRed);
       healthUpdate = true;

        if (HP <= 0) {

            StartCoroutine(death());
        }     

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

            healthUpdate = true;
            healing = true;
            GameManager.instance.FlashScreen(Color.lightGreen);
        }
        if (type == 1) {

            totalAmmo += amount;
            UpdateAmmo();
        }
    }

    public void equipGun(gunStats gun)
    {
        currGun = gun;
        shootDist = gun.shootDistance;
        shootCoolDown = gun.shootRate;
        gunNoiseLevel = gun.shootVol * 10;
       actualGun = Instantiate(gun.model, gunModel.transform);
        currGun.ammoCur = currGun.ammoMax;
     
        //gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        //gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        INoise detector = other.GetComponent<INoise>();
        if (detector != null)
        {
            detector.hearNoise();
        }
    }
    IEnumerator death()
    {
        GameManager.instance.PlayerInput.SwitchCurrentActionMap("Menus");
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(2);
        GameManager.instance.YouLose();

    }
}
