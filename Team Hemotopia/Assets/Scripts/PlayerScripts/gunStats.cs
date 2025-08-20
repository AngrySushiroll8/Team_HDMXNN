using UnityEngine;


[CreateAssetMenu]


public class gunStats : ScriptableObject
{
    public GameObject model;

    public bool isAutomtic;

    [Range(1, 100)] public int damage;
    [Range(5, 60)] public int fireDist;
    [Range(1, 6)] public int bullets;
    [Range(0.1f, 3)] public float fireRate;
    public int ammoCur;
    [Range(5, 50)] public int ammoMax;
    [Range(0.015f, 5)] public float bloomMod;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float fireVol;
    [Range(0, 30)] public int rageMeterIncrement;
}
