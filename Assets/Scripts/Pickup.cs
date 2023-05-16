using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] PickupType pickupType;

    [SerializeField] int pickupAmount;

    [Header("Ammo")]
    [SerializeField] Weapon weaponAmmoPickup;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            Player.instance.Pickup(pickupType, pickupAmount, weaponAmmoPickup);
            GameManager.instance.AddScore(GameManager.instance.scoreGetPickup);
            Destroy(gameObject);
        }
    }
}

public enum PickupType
{
    Health,
    Ammo
}