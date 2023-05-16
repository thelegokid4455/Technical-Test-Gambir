using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int health;
    [SerializeField] float deadTimer;
    int dead = 1;

    [Header("Child Rigidbodies")]
    [SerializeField] bool useRigidbodyChild;
    [SerializeField] List<Rigidbody> rigidbodyParts = new List<Rigidbody>();

    [Header("Sound")]
    [SerializeField] AudioClip soundDestroyed;

    private void Start()
    {
        if (useRigidbodyChild)
        {
            foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
            {
                rb.GetComponent<Collider>().enabled = false;
                rb.useGravity = false;
                rb.isKinematic = true;

                rigidbodyParts.Add(rb.GetComponent<Rigidbody>());
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            deadTimer -= Time.deltaTime;
            if(dead > 0)
            {
                dead = 0;
                Die();
            }
            if (deadTimer <= 0)
                Destroy(gameObject);
        }
    }

    void ApplyDamage(int dmg)
    {
        health -= dmg;
    }

    void Die()
    {
        GetComponent<AudioSource>().PlayOneShot(soundDestroyed);

        if (useRigidbodyChild)
        {
            GetComponent<Collider>().enabled = false;
            //use ragdoll
            foreach (Rigidbody rb in rigidbodyParts)
            {
                rb.GetComponent<Collider>().enabled = true;
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
        
    }
}
