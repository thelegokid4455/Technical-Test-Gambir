using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Gameplay/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Data")]
    public string weaponName;
    public WeaponType weaponType;
    public GameObject weaponProjectile;
    public bool weaponUnlimitedAmmo;
    public bool weaponIsAuto;
    public bool canReload;

    [Header("Weapon Data")]
    public float weaponInaccuracy;
    public int weaponMaxAmmoInMag;
    public int weaponMaxAmmoInBag;
    public float weaponDamage;
    public float weaponForce;
    public float weaponRange;
    public float weaponProjectileSpeed;
    public float weaponFireRate;
    public float weaponReloadTime;

    [Header("Effect")]
    public GameObject weaponHitEffect;
    public GameObject weaponBloodEffect;

    [Header("Weapon Sounds")]
    public AudioClip weaponShootSound;
    public AudioClip weaponReloadSound;
}

public enum WeaponType
{
    Melee,
    Launcher,
    Raycast
}