using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class enemyAI_sniper : MonoBehaviour, IDamage, INoise
{
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float roamAngle;
    [SerializeField] int roamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    Color[] colorOrig;

    float shootTimer;
    float roamTimer;
    float angletoPlayer;

    bool PlayerinTrigger;

    Vector3 playerDir;
    Quaternion startingDir;
    Quaternion angleDeviation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = new Color[3];
        colorOrig[0] = model.materials[0].color;
        colorOrig[1] = model.materials[1].color;
        colorOrig[2] = model.materials[2].color;

        startingDir = transform.rotation;
        angleDeviation = Quaternion.Euler(0f, roamAngle, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerinTrigger && !canSeePlayer())
        {
            roamTimer += Time.deltaTime;
            checkRoam();
        }
        else if (!PlayerinTrigger)
        {
            checkRoam();
        }
    }


    void checkRoam()
    {
        if (roamTimer >= roamPauseTime)
        {
            roam();
        }
    }

    void roam()
    {
        StartCoroutine(lookAround());
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angletoPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            //if I can see you
            if (hit.collider.CompareTag("Player") && angletoPlayer <= FOV)
            {
                roamTimer = 0;
                shoot();

                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerinTrigger = true;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }   
    }

    public void takeDamage(int amount)
    {

        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());

        }
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.materials[0].color = Color.red;
        model.materials[1].color = Color.red;
        model.materials[2].color = Color.red;
        yield return new WaitForSeconds(0.2f);
        model.materials[0].color = colorOrig[0];
        model.materials[1].color = colorOrig[1];
        model.materials[2].color = colorOrig[2];
    }

    public void hearNoise()
    {
        faceTarget();
    }

    IEnumerator lookAround()
    {
        Quaternion lookDir = startingDir * angleDeviation;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookDir, Time.deltaTime * faceTargetSpeed);
        yield return new WaitForSeconds(faceTargetSpeed);
        lookDir = lookDir * Quaternion.Euler(0f, -roamAngle, 0f);
    }
}
