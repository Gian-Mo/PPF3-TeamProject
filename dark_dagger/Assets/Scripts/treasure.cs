using TMPro;
using UnityEngine;

public class treasure : MonoBehaviour
{
    [SerializeField] gunStats gun;
    public GameObject pistolModel;
    public GameObject rifleModel;
    public GameObject sniperModel;
    public AudioClip[] pistolSound;
    public AudioClip[] rifleSound;
    public AudioClip[] sniperSound;
    public TextMeshProUGUI text;

    private bool nearby = false;
    private playerController player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gun = generateGun();
        updateGunUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (text != null)
        {
            text.transform.rotation = Quaternion.LookRotation(text.transform.position - Camera.main.transform.position);
        }
        if (nearby && Input.GetKeyDown(KeyCode.Space))
        {
            gunSwap();
        }
    }

    private gunStats generateGun()
    {
        gunStats loot = new gunStats();
        System.Random rand = new System.Random();

        int gamble = rand.Next(1, 4);
        if (gamble == 1)
        {
            loot.type = "Pistol";
            Destroy(rifleModel);
            Destroy(sniperModel);
            rifleSound = null;
            sniperSound = null;

            loot.model.GetComponent<MeshFilter>().sharedMesh = pistolModel.GetComponent<MeshFilter>().sharedMesh;
            loot.model.GetComponent<MeshRenderer>().sharedMaterial = pistolModel.GetComponent<MeshRenderer>().sharedMaterial;
            loot.shootSound = pistolSound;
            loot.shootVol = (float)(rand.NextDouble() * (0.3 - 0.1) + 0.1);
            loot.shootDamage = rand.Next(1, 4);
            loot.shootDistance = rand.Next(5, 11);
            loot.shootRate = (float)(rand.NextDouble() * (1.5 - .5) + 0.5);
            loot.ammoMax = rand.Next(10, 21);
            loot.ammoCur = loot.ammoMax;
        }
        if (gamble == 2)
        {
            loot.type = "Rifle";
            Destroy(pistolModel);
            Destroy(sniperModel);
            pistolSound = null;
            sniperSound = null;
            loot.model.GetComponent<MeshFilter>().sharedMesh = rifleModel.GetComponent<MeshFilter>().sharedMesh;
            loot.model.GetComponent<MeshRenderer>().sharedMaterial = rifleModel.GetComponent<MeshRenderer>().sharedMaterial;
            loot.shootSound = rifleSound;
            loot.shootVol = (float)(rand.NextDouble() * (0.6 - 0.3) + 0.3);
            loot.shootDamage = rand.Next(1, 3);
            loot.shootDistance = rand.Next(7, 14);
            loot.shootRate = (float)(rand.NextDouble() * (0.7 - 0.2) + 0.2);
            loot.ammoMax = rand.Next(20, 41);
            loot.ammoCur = loot.ammoMax;
        }
        if (gamble == 3)
        {
            loot.type = "Sniper";
            Destroy(rifleModel);
            Destroy(pistolModel);
            rifleSound = null;
            pistolSound = null;
            loot.model.GetComponent<MeshFilter>().sharedMesh = sniperModel.GetComponent<MeshFilter>().sharedMesh;
            loot.model.GetComponent<MeshRenderer>().sharedMaterial = sniperModel.GetComponent<MeshRenderer>().sharedMaterial;
            loot.shootSound = sniperSound;
            loot.shootVol = (float)(rand.NextDouble() * (1.0 - 0.6) + 0.6);
            loot.shootDamage = rand.Next(5, 11);
            loot.shootDistance = rand.Next(12, 21);
            loot.shootRate = (float)(rand.NextDouble() * (3.0 - 1.2) + 1.2);
            loot.ammoMax = rand.Next(3, 8);
            loot.ammoCur = loot.ammoMax;
        }

        return loot;
    }

    private void updateGunUI()
    {
        if (text != null && gun != null)
        {
            text.text = $"Type: {gun.type}\nDamage: {gun.shootDamage}\nShots per Second: {gun.shootRate:F2}\nRange: {gun.shootDistance}";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearby = true;
            player = other.GetComponent<playerController>();
            if (text != null)
                text.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearby = false;
            player = null;
            if (text != null)
                text.gameObject.SetActive(false);
        }
    }

    private void gunSwap()
    {
        if (player == null)
            return;
        gunStats hold = player.currGun;
        player.equipGun(gun);
        gun = hold;
        updateGunUI();
    }
}
