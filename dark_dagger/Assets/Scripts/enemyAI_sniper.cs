using System.Collections;
using UnityEngine;


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
    [SerializeField] Quaternion angleDeviation;

    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource shootSound;

    Color[] colorOrig;
    LineRenderer lineRenderer;

    float shootTimer;
    float angletoPlayer;

    bool PlayerinTrigger;

    Vector3 playerDir;
    Quaternion startingDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = new Color[3];
        colorOrig[0] = model.materials[0].color;
        colorOrig[1] = model.materials[1].color;
        colorOrig[2] = model.materials[2].color;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(headPos.position, playerDir, out hit);
        if (PlayerinTrigger && canSeePlayer())
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, shootPos.transform.position);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        else
        {
            lineRenderer.enabled = false;
        }
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
                shootTimer += Time.deltaTime;
                shoot();
                faceTarget();
                return true;
            }
        }
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x - 0.5f, 0, playerDir.z));
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

        if (shootTimer >= shootRate)
        {
            shootSound.Play();
            Instantiate(bullet, shootPos.position, transform.rotation);
            shootTimer = 0;
        }   
    }

    public void takeDamage(int amount)
    {
        hitSound.Play();
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
    
}
