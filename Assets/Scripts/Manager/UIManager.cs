using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] Text selectedWeaponText;
    [SerializeField] Text selectedAmmoText;

    [Header("Health")]
    [SerializeField] Text healthtext;
    [SerializeField] Image damageImage;
    float a = 0; //damage image alpha
    Color color;

    [Header("Ending")]
    [SerializeField] GameObject endMenu;
    [SerializeField] Text finalScore;

    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        color = damageImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        selectedWeaponText.text = Player.instance.weaponsList[Player.instance.selectedWeaponInt].thisWeapon.weaponName;
        selectedAmmoText.text = $"{Player.instance.weaponsList[Player.instance.selectedWeaponInt].curAmmo} / {Player.instance.weaponsList[Player.instance.selectedWeaponInt].ammoInMag}";

        healthtext.text = Player.instance.curHealth.ToString();

        damageImage.color = color;
        color.a = a;

        if (a > 0)
            a -= Time.deltaTime;
    }

    public void ApplyDamage()
    {
        a = 1;
    }

    public void GameFinish()
    {
        endMenu.SetActive(true);
        finalScore.text = "" + GameManager.instance.currentScore;
    }
}
