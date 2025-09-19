using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class treasure : MonoBehaviour
{
    [SerializeField] gunStats gun;
    public GameObject pistolModel;
    public GameObject rifleModel;
    public GameObject sniperModel;
    public AudioClip[] pistolSound;
    public AudioClip[] rifleSound;
    public AudioClip[] sniperSound;
    public TextMeshPro text;

    private bool nearby = false;
    private playerController player;
    [SerializeField] private int level = 0;

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
        if (nearby && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            gunSwap();
        }
    }

    private gunStats generateGun()
    {
        gunStats loot = ScriptableObject.CreateInstance<gunStats>();
        System.Random rand = new System.Random();

        int gamble = rand.Next(1, 4);
        if (gamble == 1)
        {
            loot.type = "Pistol";
            //Destroy(rifleModel);
            //Destroy(sniperModel);
            rifleSound = null;
            sniperSound = null;

            loot.model = pistolModel;
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
            //Destroy(pistolModel);
            //Destroy(sniperModel);
            pistolSound = null;
            sniperSound = null;
            loot.model = rifleModel;
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
            //Destroy(rifleModel);
            //Destroy(pistolModel);
            rifleSound = null;
            pistolSound = null;
            loot.model = sniperModel;
            loot.shootSound = sniperSound;
            loot.shootVol = (float)(rand.NextDouble() * (1.0 - 0.6) + 0.6);
            loot.shootDamage = rand.Next(5, 11);
            loot.shootDistance = rand.Next(12, 21);
            loot.shootRate = (float)(rand.NextDouble() * (3.0 - 1.2) + 1.2);
            loot.ammoMax = rand.Next(3, 8);
            loot.ammoCur = loot.ammoMax;
        }
        if (level != 0)
        {
            float lowMod = Mathf.Pow(0.9f, level);
            float highMod = Mathf.Pow(1.2f, level);

            loot.shootVol *= lowMod;
            loot.shootRate *= lowMod;
            loot.shootDamage *= Mathf.Max(1, Mathf.RoundToInt(loot.shootDamage * highMod));
            loot.shootDistance *= Mathf.Max(1, Mathf.RoundToInt(loot.shootDistance * highMod));
            loot.ammoMax *= Mathf.Max(1, Mathf.RoundToInt(loot.ammoMax * highMod));
            loot.ammoCur = loot.ammoMax;
        }

        return loot;
    }

    private void updateGunUI()
    {
        if (text != null && gun != null)
        {
            string adj = gunQuality(gun);
            text.text = $"Type:{adj} {gun.type}\nDamage: {gun.shootDamage}\nShots per Second: {gun.shootRate:F2}\nRange: {gun.shootDistance}";
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

    private string gunQuality(gunStats curr)
    {
        float shootValTier = 1f / curr.shootVol;
        float shootRateTier = 1f / curr.shootRate;
        float tot = shootValTier + shootRateTier + curr.shootDamage + curr.shootDistance + curr.ammoMax;
        if (tot < 35.5)
            return "Poor";
        if(tot < 43.0)
            return "Decent";
        if (tot < 50.4)
            return "Good";
        if (tot < 60.0)
            return "Great";
        return "Pristine";
    }
}
