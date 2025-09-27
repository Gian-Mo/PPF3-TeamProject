using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] AudioSource gunSound;
    [SerializeField] GameObject radarObject;
    [SerializeField] int killsForRadar;
    [SerializeField] AudioSource walk;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int ammoTotalMax;

    public gunStats currGun;
    [SerializeField] GameObject gunModel;
    public int radarKills;

    GameObject radarCurr;
    int HPOrig;
    float heighOrig;
    int speedOrig; 
    public Vector3 playerVel;

    public InputActionReference move;
    public InputActionReference shoot;
    public InputActionReference meele;
   public InputActionReference crouch; 
    public InputActionReference reload;
    public InputActionReference radar;
    public Image meleeCursor;

    Vector3 mouseDirection;

    bool shootRot;
    bool ableToShoot;
    bool ableToCrouch;
    bool inShootDist;
    bool healthUpdate;
    bool healing;
    bool ammoUpdate;
    bool radarUpdate;
    public bool canChangeCursor;
    public int totalAmmo;
    float meeleTimer;
    float shootTimer;
   
    float detectionTimer;
    float detectionDuration;

    float gunNoiseLevel;
    SphereCollider objectCollider;
    public float noiseLevel = 0;
    GameObject actualGun;
    public List<gunStats> gunList = new List<gunStats>(2);
    public int gunListPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        HPOrig = HP;
        heighOrig = controller.height;
        speedOrig = speed;
       
        meeleTimer = meeleCoolDown;
        canChangeCursor = true;
        radarCurr = null;
        detectionDuration = radarObject.GetComponent<Radar>().detectionDuration;
        objectCollider = GetComponent<SphereCollider>();
        objectCollider.radius = noiseLevel;
        equipGun(currGun);
        gunList[0].ammoCur = gunList[0].ammoMax;
        gunList[1].ammoCur = gunList[1].ammoMax;
        UpdateAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        objectCollider.radius = noiseLevel;
        Move();
        if(Time.timeScale > 0)
            selectGun();
        if (transform.position == Vector3.zero)
        {
            GameObject spawn = GameObject.FindWithTag("Start");
            if (spawn != null)
            {
                Vector3 spawnPos = spawn.transform.position;
                transform.position = new Vector3(spawnPos.x, spawnPos.y + 1.0f, spawnPos.z);
            }
        }

    }


    void Move()
    {
        meeleTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;
        if(radarUpdate && radarCurr == null)
        {
            detectionTimer += Time.deltaTime;
        }

        playerVel = move.action.ReadValue<Vector3>();
        playerVel = playerVel.normalized * speed * Time.deltaTime;
        noiseLevel = 0;
        if (move.action.IsPressed())
        {
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,Quaternion.LookRotation(playerVel.normalized),8 * Time.deltaTime);
            noiseLevel = 2;
            walk.enabled = true;
        }
        else
        {
            walk.enabled = false;
        }



        if (transform.position.y <= 1 )
        {
            controller.Move(playerVel);
        }
        else
        {
            controller.Move(new Vector3(0, -1, 0));
        }

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

        if (meeleTimer < meeleCoolDown) {

            meleeCursor.fillAmount -= (float) 1 / meeleCoolDown * Time.deltaTime;
        }

        Crouch();

       UpdatePlayerUI();
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
                    Debug.DrawRay(transform.position, mouseDirection, Color.white, 1);

                    mouseDirection = new Vector3(mouseDirection.x, 0, mouseDirection.z);
                
                    StartCoroutine(TurnPlayerWhenShoot(0.01f)); 
                    
                    //MuzzleEffect();
                    GameObject bullet = Instantiate(projectile, shootPos.position, Quaternion.LookRotation(mouseDirection));
                    Damage gunDmg = bullet.GetComponent<Damage>();
                    if (gunDmg != null && currGun != null)
                        gunDmg.setDamage(currGun.shootDamage);
                    gunSound.Play();
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
        Ray attack = new Ray();
        attack.origin = transform.position;
        attack.direction = model.transform.forward;

        if (Physics.SphereCast(attack, 0.7f, out hit, 2))
        {
            StartCoroutine(MeeleFeedBack(hit));
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(meeleDamage);

            }
        }
        else
        {
            attack.origin = attack.GetPoint(2);
            attack.direction = -model.transform.forward;            
        }
        if (Physics.SphereCast(attack, 0.7f, out hit, 1, ~LayerMask.GetMask("Player")))
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

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreLayer))
        {
     
            Physics.Raycast(ray, out hit);     
        }   

        return hit.point;
    }

    private void OnEnable()
    {
        EnableShoot(true);
        EnableCrouch(true);
        meele.action.started += Meele;
        reload.action.started += Reload;
        radar.action.started += Radar;
    }


    private void OnDisable()
    {
        EnableShoot(false);
        EnableCrouch(false);
        meele.action.started -= Meele;
        reload.action.started -= Reload;
        radar.action.started -= Radar;
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
                meleeCursor.fillAmount = 1;
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

    private void Radar(InputAction.CallbackContext context)
    {

        if (radarKills >= killsForRadar)
        {
           radarCurr = Instantiate(radarObject, transform.position, Quaternion.identity); 
            radarKills = 0;
            radarUpdate = true;
           
        }

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

    public void UpdatePlayerUI()
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

        if(radarKills >= killsForRadar) GameManager.instance.playerRadar.fillAmount = 0;

        if (radarUpdate)
        {

            if (detectionTimer > detectionDuration)
            {
                radarUpdate = false;
                detectionTimer = 0;
                GameManager.instance.playerRadar.fillAmount = 1;
            }
            else
            {

                GameManager.instance.playerRadar.fillAmount = Mathf.Lerp(GameManager.instance.playerRadar.fillAmount, detectionTimer / detectionDuration, 2 * Time.deltaTime); 
            }


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

            if (totalAmmo + amount <= ammoTotalMax)
            {
                totalAmmo += amount; 
            }
            else
            {
                totalAmmo = ammoTotalMax;
            }
                UpdateAmmo();
        }
    }

    public void equipGun(gunStats gun)
    {
        ChangeAnims(gun.type, currGun.type);
        currGun = gun;
        shootDist = gun.shootDistance;
        shootCoolDown = gun.shootRate;
        gunNoiseLevel = gun.shootVol * 75;
        if (actualGun != null) { Destroy(actualGun); }
        actualGun = Instantiate(gun.model, gunModel.transform);
        currGun.ammoCur = gun.ammoCur;
        gunSound.resource = gun.shootSound;
        gunList[gunListPos] = gun;

        GameManager.instance.updateHotBar();

    }


    void selectGun()
    {
        if (Mouse.current.scroll.ReadValue().y > 0 && gunListPos > 0)
        {
            gunList[gunListPos].ammoCur = currGun.ammoCur;
            gunListPos--;
            equipGun(gunList[gunListPos]);
            UpdateAmmo();
        }
        else if (Mouse.current.scroll.ReadValue().y < 0 && gunListPos < gunList.Count - 1)
        {
            gunList[gunListPos].ammoCur = currGun.ammoCur;
            gunListPos++;
            equipGun(gunList[gunListPos]);
            UpdateAmmo();
        }
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

   void ChangeAnims(string on, string off)
    {
            anim.SetBool(on, true);
        if (!off.Equals(on))
        {
            anim.SetBool(off, false);  
        }
        
    }

}
