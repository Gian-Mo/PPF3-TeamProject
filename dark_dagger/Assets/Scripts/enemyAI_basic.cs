using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI_basic : MonoBehaviour, IDamage, INoise
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;

    [SerializeField] List<GameObject> dropTable = new List<GameObject>();

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    [SerializeField] AudioSource hitSound;

    Color[] colorOrig;

    float shootTimer;
    float roamTimer;
    float angletoPlayer;
    float stoppingDistOrig;

    bool PlayerinTrigger;

    Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = new Color[3];
        colorOrig[0] = model.materials[0].color;
        colorOrig[1] = model.materials[1].color;
        colorOrig[2] = model.materials[2].color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLoco();

        shootTimer += Time.deltaTime;

        if (agent.remainingDistance <= 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (PlayerinTrigger && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!PlayerinTrigger)
        {
            checkRoam();
        }
    }

    void setAnimLoco()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTransSpeed));
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;

        agent.stoppingDistance = 0;

        Vector3 randPos = Random.insideUnitSphere * roamDist;
        randPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
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
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
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
            agent.stoppingDistance = 0;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    public void takeDamage(int amount)
    {
        hitSound.Play();
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());

            agent.SetDestination(GameManager.instance.player.transform.position);
        }
        if (HP <= 0)
        {
            int randomIndex = Random.Range(0, dropTable.Count);
            Instantiate(dropTable[randomIndex], transform.position, transform.rotation);

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
        agent.SetDestination(GameManager.instance.player.transform.position);
    }
}
