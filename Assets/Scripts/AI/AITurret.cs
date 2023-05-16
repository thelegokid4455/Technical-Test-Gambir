using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AITurret : AICharacter
{
    [Header("Rotation")]
    [SerializeField] GameObject rotateObject;
    [SerializeField] float rotateSpeed;
    [SerializeField] bool lockX;
    [SerializeField] bool lockY;
    [SerializeField] bool lockZ;
    private Vector3 lookPos;
    private Quaternion rotation;

    [Header("Health")]
    [SerializeField] bool isAlive;
    [SerializeField] float curHealth;
    [SerializeField] float maxHealth;
    [SerializeField] float deadBodyDisappearTime;
    float deadTime;
    [SerializeField] List<Rigidbody> rigidbodyParts = new List<Rigidbody>();
    [SerializeField] GameObject explosionObject;
    int explode = 1;

    [Header("Attack")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] GameObject muzzleFlash;

    [SerializeField] float weaponInaccuracy;
    [SerializeField] float attackDamage;
    [SerializeField] float attackForce;
    [SerializeField] float attackRate;
    float fireRate;
    [SerializeField] bool isAttacking;
    [SerializeField] GameObject weaponBloodEffect;
    [SerializeField] GameObject weaponHitEffect;

    [Header("Sound")]
    [SerializeField] AudioClip soundAttack;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        canMove = true;

        SetHealth();
    }

    void SetHealth()
    {
        curHealth = maxHealth;
        isAlive = true;
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Rigidbody>())
            {
                rigidbodyParts.Add(child.GetComponent<Rigidbody>());
            }
        }

        foreach (Rigidbody rb in rigidbodyParts)
        {
            rb.GetComponent<Collider>().enabled = false;
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public override void ApplyTarget()
    {
        if(isAlive)
        {
            if (curTarget)
            {
                //distance
                var distance = Vector3.Distance(curTarget.position, transform.position);

                if (distance <= minDistanceToTarget)// && distance < chaseDistance)
                {
                    lookPos = curTarget.position - transform.position;
                    //rotate
                    if (lockX)
                        lookPos.x = 0;
                    if (lockY)
                        lookPos.y = 0;
                    if (lockZ)
                        lookPos.z = 0;

                    rotation = Quaternion.LookRotation(lookPos);
                    if (rotation != Quaternion.Euler(Vector3.zero))
                    {
                        if (canSeeTarget)
                        {
                            rotateObject.transform.rotation = Quaternion.Slerp(rotateObject.transform.rotation, rotation, Time.deltaTime * rotateSpeed);
                            Attack();
                        }
                    }
                }
            }
        }
        
    }

    public override void ApplyMovement()
    {
        if (isAlive)
        {
            fireRate -= Time.deltaTime;

            if (curTarget)
            {

            }
        }
        else
        {
            //destroy after a few seconds
            deadTime += Time.deltaTime;

            if (deadTime >= deadBodyDisappearTime)
                Destroy(gameObject);
        }

    }

    public override void ApplyAnimation()
    {

    }

    public override void ApplyHealth()
    {
        if (curHealth <= 0)
        {
            Die();
        }
        else
        {
            isAlive = true;
        }
    }


    [ContextMenu("Damage Player")]
    public void DamageCharacter()
    {
        ApplyDamage(10);
    }

    public void Die()
    {
        isAlive = false;
        canMove = false;
        GetComponent<Collider>().enabled = false;
        if(explode > 0)
        {
            Instantiate(explosionObject, transform.position, transform.rotation);
            explode = 0;
        }

        //uyse ragdoll
        foreach (Rigidbody rb in rigidbodyParts)
        {
            rb.GetComponent<Collider>().enabled = true;
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        //remove from list
        if (GameplayManager.instance.allEnemies.Contains(gameObject))
        {
            GameManager.instance.AddScore(GameManager.instance.scoreKillTurret);
            GameplayManager.instance.allEnemies.Remove(gameObject);
        }
    }

    public virtual void ApplyDamage(float dmg)
    {
        curHealth -= dmg;
    }
    public void Attack()
    {
        if (isAttacking)
            return;

        if (fireRate <= 0)
        {
            fireRate = attackRate;
            FireOneShot();
            //play sound
            GetComponent<AudioSource>().PlayOneShot(soundAttack);
            if (muzzleFlash)
                muzzleFlash.GetComponent<ParticleSystem>().Play();
        }
    }

    void FireOneShot()
    {
        Vector3 dir = bulletSpawn.TransformDirection(new Vector3(Random.Range(-weaponInaccuracy, weaponInaccuracy), Random.Range(-weaponInaccuracy, weaponInaccuracy), 1));
        Vector3 pos = bulletSpawn.position;
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, chaseDistance, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(attackForce * dir, hit.point);

            hit.transform.SendMessage("ApplyDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

            GameObject def = null;
            if (hit.transform.CompareTag("Player"))
            {
                def = Instantiate(weaponBloodEffect, contact, rot);
            }
            else
            {
                def = Instantiate(weaponHitEffect, contact, rot);

            }

            def.transform.localPosition += .02f * hit.normal;
            def.transform.localScale = new Vector3(rScale, rScale, rScale);
            def.transform.parent = hit.transform;
        }
    }

}
