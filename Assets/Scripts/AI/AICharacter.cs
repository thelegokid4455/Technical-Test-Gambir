using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : MonoBehaviour
{
    public AIType characterType;

    [Header("Target")]
    public Transform curTarget;
    public LayerMask seeLayer;
    public bool canSeeTarget;
    public float minDistanceToTarget;
    public float chaseDistance;


    [Header("Movement")]
    public bool canMove;
    public CharacterController controller;
    public int curMoveMode; //-1 reverse, 0 idle, 1 chase
    public float runSpeed;
    public float walkSpeed;
    public float crouchSpeed;
    public float rotDelay;

    public float jumpSpeed;
    public float gravity;
    public float pushPower;

    [Header("Footsteps")]
    public float footStepLengthWalk;
    public float footStepLenghtRun;
    float step;
    float stepTimer;
    public AudioClip footStepSound;


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.hasStarted)
            return;

        ApplyHealth();
        ApplyTarget();
        CanSeeTarget();
        ApplyAnimation();
        Footstep();
        
    }
    void FixedUpdate()
    {
        if (!GameManager.instance.hasStarted)
            return;

        ApplyMovement();
    }

    public void CanSeeTarget()
    {
        if (!curTarget)
            return;

        Vector3 dir = curTarget.transform.position - transform.position;
        Vector3 pos = transform.position;
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, chaseDistance, seeLayer))
        {
            if (hit.transform == curTarget)
                canSeeTarget = true;
            else
                canSeeTarget = false;
        }
    }

    public virtual void ApplyTarget() { }
    public virtual void ApplyHealth() { }
    public virtual void ApplyMovement() { }
    public virtual void ApplyAnimation() { }
    public virtual void ApplyDamage() { }

    public void Footstep()
    {
        if(characterType == AIType.Character)
        {
            if (canMove)
            {
                if (controller.velocity.magnitude >= (walkSpeed - 0.1f) && controller.velocity.magnitude < (runSpeed - 1)) //walk
                {
                    step = footStepLengthWalk;
                }
                else if (controller.velocity.magnitude >= (runSpeed - 1)) //run
                {
                    step = footStepLenghtRun;
                }
                else if (controller.velocity.magnitude < (walkSpeed + 0.1f))
                {
                    step = footStepLengthWalk * 2;
                }

                //sounds
                if (controller.velocity.magnitude > 0)
                    stepTimer -= Time.deltaTime;

                if (stepTimer <= 0)
                {
                    stepTimer = step;
                    GetComponent<AudioSource>().PlayOneShot(footStepSound);
                }
            }
            else
            {
                stepTimer = step;
            }
        }
        
    }
    public void SetTarget(Transform target)
    {
        if (target == curTarget)
            return;

        curTarget = target;
    }
}

public enum AIType
{ 
    Character,
    Turret
}