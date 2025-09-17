using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 15)] public int shootDamage;
    [Range(1, 1000)] public int shootDistance;
    [Range(0.1f, 3)] public float shootRate;
    public int ammoCur;
    [Range(5, 50)] public int ammoMax;
    public string type;
    
    public ParticleSystem muzzleFlash;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootVol;
}
