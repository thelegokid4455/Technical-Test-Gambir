using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] GameObject explosionObject;

    private void Update()
    {
        if (health <= 0)
            Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void ApplyDamage(int dmg)
    {
        health -= dmg;
    }

    void Explode()
    {
        Instantiate(explosionObject, transform.position, transform.rotation);
        GameManager.instance.AddScore(GameManager.instance.scoreDestroyMine);
        Destroy(gameObject);
    }
}
