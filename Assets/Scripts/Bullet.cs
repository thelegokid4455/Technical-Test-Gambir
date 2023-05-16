using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Data")]
    public bool isImpact; //explodes at impact
    public bool isThrust; //adds thrust to projectile for bullets, rockets etc
    public bool isChase; //chases a target if available for seeking rockets

    public float bulletSpeed;
    public float bulletDamage;
    [SerializeField] GameObject bulletExplosion;
    public Transform target;

    [Header("Positioning")]
    [SerializeField] bool lockX;
    [SerializeField] bool lockY;
    [SerializeField] bool lockZ;
    [SerializeField] float rotSpeed;

    private Vector3 lookPos;
    private Quaternion rotation;

    [SerializeField] float timeToExplode;
    float curBulletTime;

    void Update()
    {
        curBulletTime += 1f * Time.deltaTime;

        if(curBulletTime > timeToExplode)
        {
            Explode();
        }
    }

    private void FixedUpdate()
    {
        if (target)
            lookPos = target.position - transform.position;

        //if bullet has acceleration
        if (isThrust)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed);
        }


        //if bullet is heat seekign
        if (isChase)
        {
            //lock axises for easier control
            if (lockX)
                lookPos.x = 0;
            if (lockY)
                lookPos.y = 0;
            if (lockZ)
                lookPos.z = 0;

            rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotSpeed);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(isImpact)
        {
            if (!other.CompareTag("DontHit"))
            {
                other.gameObject.SendMessage("ApplyDamage", bulletDamage, SendMessageOptions.DontRequireReceiver);
                Explode();

            }
        }
    }

    void Explode()
    {
        if(bulletExplosion)
        {
            GameObject explosion = Instantiate(bulletExplosion, transform.position, transform.rotation);
            explosion.GetComponent<explosion>().explosionDamage = bulletDamage;
        }
        Destroy(gameObject);
    }
}
