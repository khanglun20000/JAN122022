using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHoldWeapon : MonoBehaviour
{
    public delegate void OnPickUpWeapon(WeaponData weaponData);
    public OnPickUpWeapon pickedUp;
    public WeaponData wpData;

    [SerializeField] GameObject OpenBuffMenuButton;

    public bool IsWeaponPickedUp = false;

    public static UpdateHoldWeapon instance;
    [SerializeField] Transform weapon;

    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController animatorCtrlr;
    [SerializeField] PlayerMovement PM;
    [SerializeField] SpriteRenderer SR;

    bool canFlip = false;

    [SerializeField] PlayerMeleeAttack PMA;
    [SerializeField] PlayerRangedAttack PRA;
    [SerializeField] WeaponCtrlr WC;
    [SerializeField] Transform bulletPos;
    [SerializeField] PlayerBuffController PBC;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pickedUp += UpdateWeapon; // called in PickUpWeapon
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DungeonGenerator.instance.RoomPlayerIn.GetComponent<RoomController>().RoomDeactivated();
        }
        if(canFlip)
        {
            Flip();
        }
        
    }

    void Flip()
    {
        switch (wpData.combatStyle)
        {
            case CombatStyle.sword:
                if (PM.facingRight)
                {
                    SR.flipY = false;
                }
                else
                {
                    SR.flipY = true;
                }
                break;
            case CombatStyle.staff:
                if (PM.facingRight)
                {
                    weapon.localRotation = Quaternion.Euler(0, 0, 45);
                }
                else
                {
                    weapon.localRotation = Quaternion.Euler(0, 180, 45);
                }
                break;
        }
    }

    public void UpdateWeapon(WeaponData weaponData)
    {
        if (!IsWeaponPickedUp)
        {
            OpenBuffMenuButton.SetActive(true);
            IsWeaponPickedUp = true;
        }

        wpData = weaponData;
        animatorCtrlr = wpData.animator;
        animator.runtimeAnimatorController = animatorCtrlr;
        animator.SetBool("isPickedUp", true);

        ChangeTransformByWeapon();

        canFlip = wpData.combatStyle != CombatStyle.bow;
    }

    private void ChangeTransformByWeapon()
    {
        switch (wpData.combatStyle)
        {
            case CombatStyle.sword:
                weapon.localRotation = Quaternion.Euler(0, 0, 90);
                weapon.localPosition = new Vector3(0, 5, 0);
                PMA.enabled = true;
                PBC.attack = PMA;
                PMA.WpData = wpData as SwordData;
                break;
            case CombatStyle.bow:
                weapon.localRotation = Quaternion.Euler(0,0,135);
                weapon.localPosition = new Vector3(0, 2, 0);
                PRA.enabled = true;
                PBC.attack = PRA;
                PRA.WpData = wpData as RangedWeaponData;
                break;
            case CombatStyle.staff:
                weapon.localPosition = new Vector3(0, 2, 0);
                bulletPos.localPosition = new Vector3(0, 9, 0);
                PRA.enabled = true;
                PBC.attack = PRA;
                PRA.WpData = wpData as RangedWeaponData;
                break;
        }
    }
}
