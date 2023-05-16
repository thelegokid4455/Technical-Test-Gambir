using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Health")]
    bool isAlive;
    public float maxHealth;
    public float curHealth;
    [SerializeField] AudioClip hitSound;
    int dead = 1;

    [Header("Animation")]
    public GameObject animObject;
    public Animator anim;
    float animTargetLayer;
    [SerializeField] float animTargetLayerSpeed;
    [SerializeField] List<Rigidbody> rigidbodyParts = new List<Rigidbody>();

    [Header("Movement")]
    float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    [SerializeField] float movementSharpness;

    //public float curSpeed;
    public float rotSpeed;
    float xInput;
    float yInput;

    float strafeParameter;
    Vector2 strafeParameterXZ;

    public GameObject PlayeRot;

    public float gravity = 25f;

    private CharacterController controller;
    private CollisionFlags flags;
    [HideInInspector]
    public Vector4 moveDirection;
    private bool grounded = false;

    public bool canMove = true;


    [Header("Attack")]
    public LayerMask layerMask;
    bool isKnifing;
    bool isAiming;
    float fireRate;

    [Header("Weapons")]
    public Transform bulletSpawn;
    public int selectedWeaponInt;
    public PlayerWeapon meleeWeapon;
    public PlayerWeapon[] weaponsList;
    [SerializeField] AudioClip pickupSound;

    [Header("Reload")]
    bool isReloading;
    float reloadTIme;

    [Header("Sounds")]
    [SerializeField] AudioClip soundFootstep;
    [SerializeField] float footStepRateWalk;
    [SerializeField] float footStepRateRun;
    float step;
    [SerializeField] AudioClip soundFlashlight;
    [SerializeField] AudioClip soundNVG;

    [Header("Lighting")]
    [SerializeField] Light flashLight;
    [SerializeField] GameObject nightVision;

    public static Player instance;

    private void Awake()
    {
        instance = this;
        transform.position += new Vector3(0, 2, 0);
        //controls.devices = new InputDevice[] { Gamepad.all[0] };

    }

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        canMove = true;

        SetHealth();
        SetWeaponData();
    }

    void SetHealth()
    {
        isAlive = true;
        curHealth = maxHealth;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb != GetComponent<Rigidbody>())
            {
                rb.GetComponent<Collider>().enabled = false;
                rb.useGravity = false;
                rb.isKinematic = true;
                rigidbodyParts.Add(rb.GetComponent<Rigidbody>());
            }
        }
    }

    void SetWeaponData()
    {

        selectedWeaponInt = 0;

        for (int i = 0; i < weaponsList.Length; i++)
        {
            weaponsList[i].curAmmo = weaponsList[i].thisWeapon.weaponMaxAmmoInMag;
            weaponsList[i].ammoInMag = weaponsList[i].thisWeapon.weaponMaxAmmoInBag;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.hasStarted)
            return;

        //attack
        fireRate -= Time.deltaTime * 1;
        if (fireRate <= 0)
        {
            if (isKnifing)
                isKnifing = false;
        }

        if (isAlive)
        {
            //inputs
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (!isKnifing)
            {
                if (weaponsList[selectedWeaponInt].thisWeapon.weaponIsAuto)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        InputAttack();
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        InputAttack();
                    }
                }


                if (Input.GetButton("Fire2"))
                {
                    isAiming = true;
                }
                else
                {
                    isAiming = false;
                }

                if (Input.GetButtonDown("Reload"))
                {
                    if (weaponsList[selectedWeaponInt].thisWeapon.canReload)
                        Reload();
                }
            }

            //lights
            if(Input.GetButtonDown("Flashlight"))
            {
                GetComponent<AudioSource>().PlayOneShot(soundFlashlight);
                flashLight.enabled = !flashLight.enabled;
            }

            if (Input.GetButtonDown("NVG"))
            {
                GetComponent<AudioSource>().PlayOneShot(soundNVG);
                nightVision.SetActive(!nightVision.activeSelf);
            }

            //weapons
            if (!isReloading)
                selectedWeaponInt += (int)Input.mouseScrollDelta.y;

            if (selectedWeaponInt > weaponsList.Length - 1)
                selectedWeaponInt = 0;
            if (selectedWeaponInt < 0)
                selectedWeaponInt = weaponsList.Length - 1;

            //reload
            if (isReloading)
            {
                reloadTIme -= Time.deltaTime;
                if (reloadTIme <= 0)
                {
                    isReloading = false;
                    DoneReload();
                }
            }

            //health
            if (curHealth > maxHealth)
                curHealth = maxHealth;

            if (curHealth <= 0)
            {
                if(dead > 0)
                {
                    dead = 0;
                    Die();
                }
            }

            if (grounded)
            {
                if (canMove)
                {
                    if (isAiming)
                    {
                        //aim
                        if (animTargetLayer < 1)
                            animTargetLayer += Time.deltaTime * animTargetLayerSpeed;

                        //speed
                        moveSpeed = walkSpeed;
                    }
                    else
                    {
                        if (isReloading)
                        {
                            if (animTargetLayer < 1)
                                animTargetLayer += Time.deltaTime * animTargetLayerSpeed;
                        }
                        else
                        {
                            if (animTargetLayer > 0)
                                animTargetLayer -= Time.deltaTime * animTargetLayerSpeed;
                        }
                        moveSpeed = runSpeed;
                    }

                    //For normalized vectors Dot returns 1 if they point in exactly the same direction, -1 if they point in completely opposite directions and zero if the vectors are perpendicular.
                    var forwardsAmount = Vector3.Dot(PlayeRot.transform.forward, moveDirection);
                    var sideAmount = Vector3.Dot(PlayeRot.transform.right, moveDirection);

                    Vector2 inputDir = new Vector3(sideAmount, forwardsAmount);
                    strafeParameterXZ = Vector3.Lerp(strafeParameterXZ, inputDir, movementSharpness * Time.deltaTime);

                    //animation
                    anim.SetLayerWeight(1, animTargetLayer);
                    anim.SetFloat("Speed", moveSpeed);
                    anim.SetFloat("StrafingX", strafeParameterXZ.x);
                    anim.SetFloat("StrafingZ", strafeParameterXZ.y);


                    //footsteps
                    if (controller.velocity.magnitude > 0)
                    {
                        step += Time.deltaTime;
                        if (step >= (isAiming ? footStepRateWalk : footStepRateRun))
                        {
                            GetComponent<AudioSource>().PlayOneShot(soundFootstep);
                            step = 0;
                        }
                    }
                    else
                    {
                        step = 0;
                    }
                }
            }

        }

    }

    private void LateUpdate()
    {

    }

    void FixedUpdate()
    {
        if (!GameManager.instance.hasStarted)
            return;

        controller = GetComponent<CharacterController>();
        flags = controller.Move(moveDirection * Time.deltaTime);

        if(isAlive)
        {
            if (grounded && canMove)
            {
                if (!isKnifing)
                {
                    moveDirection = transform.right * xInput + transform.forward * yInput;
                    moveDirection = moveDirection.normalized * moveSpeed;

                    //Player Rotation
                    Plane PlayerPlane = new Plane(Vector3.up, transform.position);
                    Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                    float hitDist;

                    if (isAiming)
                    {
                        if (PlayerPlane.Raycast(ray, out hitDist))
                        {
                            Vector3 targetPoint = ray.GetPoint(hitDist);
                            Quaternion targetRot = Quaternion.LookRotation(targetPoint - transform.position);
                            targetRot.x = 0;
                            targetRot.z = 0;
                            PlayeRot.transform.rotation = Quaternion.Slerp(PlayeRot.transform.rotation, targetRot, rotSpeed * Time.deltaTime);

                            //adds aiming mouse pos as target so it will look at that direction
                            Vector3 aimPos = targetPoint;
                            aimPos.x = Mathf.Clamp(aimPos.x, transform.position.x + 0, transform.position.x + 5);
                            aimPos.y = Mathf.Clamp(aimPos.y, transform.position.y + 0, transform.position.y + 5);
                            aimPos.z = Mathf.Clamp(aimPos.z, transform.position.z + 0, transform.position.z + 5);

                            PlayerCamera.instance.aimTarget.position = aimPos;
                        }
                    }
                    else
                    {
                        if (moveDirection != Vector4.zero)
                        {
                            PlayeRot.transform.rotation = Quaternion.Slerp(PlayeRot.transform.rotation, Quaternion.LookRotation(moveDirection), rotSpeed * Time.deltaTime);
                        }

                        PlayerCamera.instance.aimTarget.position = transform.position;

                    }
                }
                else
                {
                    moveDirection = Vector4.zero;
                }

            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }

    //checks for attack input
    void InputAttack()
    {
        if (isReloading)
            return;

        if(fireRate<= 0)
        {
            if (isAiming)
            {
                if(weaponsList[selectedWeaponInt].curAmmo > 0)
                    Shoot();

            }
            else //hit
            {
                Punch();
            }
        }
        else
        {
            return;
        }
        
    }

    void Shoot()
    {
        if(weaponsList[selectedWeaponInt].thisWeapon.weaponType == WeaponType.Launcher)
        {
            FireOneProjectile();
        }
        else
        {
            FireOneShot(weaponsList[selectedWeaponInt].thisWeapon.weaponDamage, weaponsList[selectedWeaponInt].thisWeapon.weaponForce, weaponsList[selectedWeaponInt].thisWeapon.weaponRange);
            if (weaponsList[selectedWeaponInt].muzzleFlash)
                weaponsList[selectedWeaponInt].muzzleFlash.GetComponent<ParticleSystem>().Play();
        }

        anim.SetTrigger("Fire");

        //reduce ammo
        if(!weaponsList[selectedWeaponInt].thisWeapon.weaponUnlimitedAmmo)
            weaponsList[selectedWeaponInt].curAmmo--;
        //reset fire rate
        fireRate = weaponsList[selectedWeaponInt].thisWeapon.weaponFireRate;
        //play sound
        GetComponent<AudioSource>().PlayOneShot(weaponsList[selectedWeaponInt].thisWeapon.weaponShootSound);

        if((weaponsList[selectedWeaponInt].curAmmo <= 0 && !(weaponsList[selectedWeaponInt].thisWeapon.canReload)))
        {
            CalculateAmmo();
        }
    }

    void Punch()
    {
        isKnifing = true;
        anim.SetTrigger("Knife");
        FireOneShot(meleeWeapon.thisWeapon.weaponDamage, meleeWeapon.thisWeapon.weaponForce, meleeWeapon.thisWeapon.weaponRange);
        //reset fire rate
        fireRate = meleeWeapon.thisWeapon.weaponFireRate;
        //play sound
        GetComponent<AudioSource>().PlayOneShot(meleeWeapon.thisWeapon.weaponShootSound);
    }

    void FireOneShot(float damage, float force, float range)
    {
        Vector3 dir = bulletSpawn.TransformDirection(new Vector3(Random.Range(-weaponsList[selectedWeaponInt].thisWeapon.weaponInaccuracy, weaponsList[selectedWeaponInt].thisWeapon.weaponInaccuracy), Random.Range(-weaponsList[selectedWeaponInt].thisWeapon.weaponInaccuracy, weaponsList[selectedWeaponInt].thisWeapon.weaponInaccuracy), 1));
        Vector3 pos = bulletSpawn.position;
        RaycastHit hit;

        if (Physics.Raycast(pos, dir, out hit, range, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(force * dir, hit.point);

            GameObject def = null;

            hit.transform.SendMessage("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            if (hit.transform.CompareTag("Enemy"))
            {
                def = Instantiate(weaponsList[selectedWeaponInt].thisWeapon.weaponBloodEffect, contact, rot);
            }
            else
            {
                def = Instantiate(weaponsList[selectedWeaponInt].thisWeapon.weaponHitEffect, contact, rot);

            }

            def.transform.localPosition += .02f * hit.normal;
            def.transform.localScale = new Vector3(rScale, rScale, rScale);
            def.transform.parent = hit.transform;
        }
    }

    void FireOneProjectile()
    {
        GameObject newBullet = Instantiate(weaponsList[selectedWeaponInt].thisWeapon.weaponProjectile, bulletSpawn.position, bulletSpawn.rotation);
        newBullet.GetComponent<Bullet>().bulletDamage = weaponsList[selectedWeaponInt].thisWeapon.weaponDamage;
        if (newBullet.GetComponent<Bullet>().isThrust)
        {
            newBullet.GetComponent<Bullet>().bulletSpeed = weaponsList[selectedWeaponInt].thisWeapon.weaponProjectileSpeed;
        }
        else
        {
            newBullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * weaponsList[selectedWeaponInt].thisWeapon.weaponProjectileSpeed);
        }
        Physics.IgnoreCollision(GetComponent<Collider>(), newBullet.GetComponent<Collider>());
    }

    void Reload()
    {
        if (isReloading || weaponsList[selectedWeaponInt].curAmmo >= weaponsList[selectedWeaponInt].thisWeapon.weaponMaxAmmoInMag || weaponsList[selectedWeaponInt].ammoInMag <= 0)
            return;

        isReloading = true;
        anim.SetTrigger("Reload");

        GetComponent<AudioSource>().PlayOneShot(weaponsList[selectedWeaponInt].thisWeapon.weaponReloadSound);

        reloadTIme = weaponsList[selectedWeaponInt].thisWeapon.weaponReloadTime;
    }

    void DoneReload()
    {

        CalculateAmmo();
        isReloading = false;
    }

    void CalculateAmmo()
    {
        int ammoUsed = weaponsList[selectedWeaponInt].thisWeapon.weaponMaxAmmoInMag - weaponsList[selectedWeaponInt].curAmmo;
        weaponsList[selectedWeaponInt].curAmmo += ammoUsed;
        weaponsList[selectedWeaponInt].ammoInMag -= ammoUsed;
        if (weaponsList[selectedWeaponInt].ammoInMag < 0)
        {
            int ammoMinus = weaponsList[selectedWeaponInt].ammoInMag;
            weaponsList[selectedWeaponInt].ammoInMag = 0;
            weaponsList[selectedWeaponInt].curAmmo -= -ammoMinus;
        }
    }


    public void ApplyDamage(int dmg)
    {
        curHealth -= dmg;
        GetComponent<AudioSource>().PlayOneShot(hitSound);

        UIManager.instance.ApplyDamage();
    }
    public void AddHealth(int amount)
    {
        curHealth += amount;
    }

    public void Pickup(PickupType type, int amount, Weapon wep)
    {
        if(type == PickupType.Health)
        {
            AddHealth(amount);
        }
        else
        {
            for (int i = 0; i < weaponsList.Length; i++)
            {
                if (weaponsList[i].thisWeapon == wep)
                {
                    weaponsList[i].ammoInMag += amount;
                }
            }
        }

        GetComponent<AudioSource>().PlayOneShot(pickupSound);
    }

    public void Die()
    {
        isAlive = false;

        //uyse ragdoll
        foreach (Rigidbody rb in rigidbodyParts)
        {
            rb.GetComponent<Collider>().enabled = true;
            anim.enabled = false;
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        MenuManager.instance.GameLoadLevel(SceneManager.GetActiveScene().name);
        //Destroy(gameObject);
    }

}

[System.Serializable]
public struct PlayerWeapon
{
    public Weapon thisWeapon;
    public int curAmmo;
    public int ammoInMag;
    public GameObject muzzleFlash;
}