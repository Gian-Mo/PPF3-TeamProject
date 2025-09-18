using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;


public class Damage : MonoBehaviour
{
    enum damageType { moving, stationary, homing };
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] float destroyTime;
    [SerializeField] int speed;
    public ParticleSystem hitEffect;


    bool isDamaging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);
            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (type == damageType.homing)
        {
            rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }

    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.isTrigger) return;

          
        IDamage dmg = collision.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            if (hitEffect != null) { 
               ParticleSystem hit = Instantiate(hitEffect,transform.position, transform.rotation);
                hit.transform.Translate(new Vector3(0, 0, -1));
            }

            Destroy(gameObject);
        }
    }

    public void setDamage(int dmg)
    {
        damageAmount = dmg;
    }




}
