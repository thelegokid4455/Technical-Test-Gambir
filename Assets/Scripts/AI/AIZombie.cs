using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AIZombie : AICharacter
{
    [Header("Movement")]
    private float curSpeed;
    private Vector3 moveDirection = Vector4.zero;
    private bool isGrounded = false;
    private CollisionFlags flags;


    [Header("Health")]
    [SerializeField] bool isAlive;
    [SerializeField] float curHealth;
    [SerializeField] float maxHealth;
    [SerializeField] float deadBodyDisappearTime;
    float deadTime;
    [SerializeField] List<Rigidbody> rigidbodyParts = new List<Rigidbody>();
    int dead = 1;

    [Header("Attack")]
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] float attackDamage;
    [SerializeField] float attackForce;
    [SerializeField] float attackRate;
    float attackTimer;
    [SerializeField] bool isAttacking;
    [SerializeField] GameObject weaponBloodEffect;

    [Header("Animation")]
    public Animator animator;

    [Header("Sound")]
    [SerializeField] AudioClip soundAttack;
    [SerializeField] AudioClip soundDie;

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

        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
        {
            rb.GetComponent<Collider>().enabled = false;
            rb.useGravity = false;
            rb.isKinematic = true;

            rigidbodyParts.Add(rb.GetComponent<Rigidbody>());
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

                if (distance > minDistanceToTarget)
                {
                    if (!isAttacking)
                    {
                        if (canSeeTarget)
                            curMoveMode = 1;
                        else
                            curMoveMode = 0;
                    }
                }
                else if (distance <= minDistanceToTarget)// && distance < chaseDistance)
                {
                    curMoveMode = 0;
                    if (canSeeTarget)
                    {
                        Attack();
                    }

                }
                /*
                if (distance < minDistanceToTarget)
                {
                    if (!isAttacking)
                        curMoveMode = -1;
                }
                */
            }
            else
            {
                curMoveMode = 0;
            }
        }
        
    }

    public override void ApplyMovement()
    {
        if(isAlive)
        {
            if (isGrounded)
            {
                //moving
                if (canMove)
                {
                    if (curTarget)
                    {
                        if (canSeeTarget)
                        {
                            if (curMoveMode == 1)
                            {
                                curSpeed = walkSpeed;
                            }
                            else if (curMoveMode == -1)
                            {
                                curSpeed = -walkSpeed;

                            }
                            else // 0
                            {
                                curSpeed = 0;
                            }
                            //move
                            moveDirection.x = (transform.forward.x * curSpeed);
                            moveDirection.z = (transform.forward.z * curSpeed);

                            //rotate
                            var targetPos = new Vector3(curTarget.position.x, transform.position.y, curTarget.position.z);
                            transform.DOLookAt(targetPos, rotDelay);
                        }
                        else
                        {
                            curMoveMode = 0;
                            moveDirection = Vector4.zero;
                        }
                    }
                }
                else
                {
                    moveDirection.x = 0;
                    moveDirection.z = 0;
                    curSpeed = walkSpeed;
                }

                moveDirection.y -= gravity * Time.deltaTime;

            }
            else
            {

                moveDirection.y -= gravity * Time.deltaTime;
            }

            flags = controller.Move(moveDirection * Time.deltaTime);
            isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
            //isGrounded = controller.isGrounded;
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
        if(canMove)
        {
            animator.SetFloat("Speed", curMoveMode);
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    public override void ApplyHealth()
    {
        if (curHealth <= 0)
        {
            if (dead > 0)
            {
                dead = 0;
                Die();
            }
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

        //disable colliders
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;

        //use ragdoll
        foreach (Rigidbody rb in rigidbodyParts)
        {
            rb.GetComponent<Collider>().enabled = true;
            animator.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        //remove from list
        if (GameplayManager.instance.allEnemies.Contains(gameObject))
        {
            GameManager.instance.AddScore(GameManager.instance.scoreKillZombie);
            GameplayManager.instance.allEnemies.Remove(gameObject);
        }

        StopAllCoroutines();
        GetComponent<AudioSource>().PlayOneShot(soundDie);
        dead = 0;

    }

    public virtual void ApplyDamage(float dmg)
    {
        curHealth -= dmg;
    }
    public void Attack()
    {
        if (isAttacking)
            return;

        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        //play sound

        yield return new WaitForSeconds(attackRate/2);

        //send raycast
        FireOneShot();
        GetComponent<AudioSource>().PlayOneShot(soundAttack);

        yield return new WaitForSeconds(attackRate / 2);
        isAttacking = false;
    }

    void FireOneShot()
    {
        Vector3 dir = bulletSpawn.TransformDirection(new Vector3(0, 0, 1));
        Vector3 pos = bulletSpawn.position;
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, minDistanceToTarget, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(attackForce * dir, hit.point);

            GameObject def = null;

            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.SendMessage("ApplyDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
                def = Instantiate(weaponBloodEffect, contact, rot);
            }
            else
            {
                //hit.transform.SendMessage("ApplyDamage", meleeWeapon.thisWeapon.weaponDamage, SendMessageOptions.DontRequireReceiver);

            }

            def.transform.localPosition += .02f * hit.normal;
            def.transform.localScale = new Vector3(rScale, rScale, rScale);
            def.transform.parent = hit.transform;
        }
    }

}
