using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public BuffData buffData;
    public abstract BuffName BuffName { get; }
    public abstract string AdditionalPath { get; }
    public abstract void ApplyBuff();

    protected string BuffDataPath = "Database/BuffData/";

    public void GetBuffDataFromPath()
    {
        buffData = Resources.Load<BuffData>(BuffDataPath);
    }
    protected Buff()
    {
        BuffDataPath += AdditionalPath;
        BuffDataPath += BuffName.ToString();
    }
}

public enum BuffName
{
    IncreaseDamage,
    IncreaseMaxHealth,
    IncreaseBulletShotPerClick,
    IncreaseBulletPerShot,
    IncreaseBounceCount,
    RankUpWeapon
}

public abstract class WhiteBuff : Buff
{
    public override string AdditionalPath => "WhiteBuff/";
}

public abstract class BlueBuff : Buff
{
    public override string AdditionalPath => "BlueBuff/";
}

public abstract class YellowBuff : Buff
{
    public override string AdditionalPath => "YellowBuff/";
}

public abstract class RedBuff : Buff
{
    public override string AdditionalPath => "RedBuff/";
}

public class IncreaseDamage : WhiteBuff
{
    public override BuffName BuffName => BuffName.IncreaseDamage;

    int damageToAdd = 10;

    public override void ApplyBuff()
    {
        Abs_Attack attack = PlayerBuffController.instance.attack;
        attack.WeaponDamage += damageToAdd;
    }
}

public class IncreaseMaxHealth : BlueBuff
{
    public override BuffName BuffName => BuffName.IncreaseMaxHealth;

    int healthToAdd = 10;

    public override void ApplyBuff()
    {
        HealthSystem healthSystem = PlayerStatController.instance.GetHealthSystem();
        healthSystem.MaxHealth += healthToAdd;
    }
}

public class IncreaseBulletShotPerClick : RedBuff
{
    public override BuffName BuffName => BuffName.IncreaseBulletShotPerClick;

    public override void ApplyBuff()
    {
        Abs_RangedAttack attack = PlayerBuffController.instance.attack as Abs_RangedAttack;
        attack.NumberOfBulletsPattern++;
    }
}

public class IncreaseBulletPerShot : YellowBuff
{
    public override BuffName BuffName => BuffName.IncreaseBulletPerShot;

    public override void ApplyBuff()
    {
        Abs_RangedAttack attack = PlayerBuffController.instance.attack as Abs_RangedAttack;
        attack.BulletsEachPattern++;
    }
}

public class IncreaseBounceCount : WhiteBuff
{
    public override BuffName BuffName => BuffName.IncreaseBounceCount;

    public override void ApplyBuff()
    {
        Abs_RangedAttack attack = PlayerBuffController.instance.attack as Abs_RangedAttack;
        attack.bounceCount++;
    }
}

public class RankUpWeapon : RedBuff
{
    public override BuffName BuffName => BuffName.RankUpWeapon;
    static int currentLevel = 2;
    string wpDataPath = "Database/StartingWeaponData/";
    public override void ApplyBuff()
    {
        UpdateHoldWeapon.instance.UpdateWeapon(ChooseRankUpWp());
    }

    WeaponData ChooseRankUpWp()
    {
        WeaponData currentWpData = UpdateHoldWeapon.instance.wpData;
        WeaponData[] wpDatasOfLevel;
        wpDataPath += "Level" + currentLevel.ToString() + "_";
        switch (currentWpData.combatStyle)
        {
            case CombatStyle.bow:
                wpDataPath +=  "Bow";
                break;
            case CombatStyle.sword:
                wpDataPath += "Sword";
                break;
            case CombatStyle.staff:
                wpDataPath += "Staff";
                break;
        }
        Debug.Log(wpDataPath);
        wpDatasOfLevel = Resources.LoadAll<WeaponData>(wpDataPath);
        currentWpData = wpDatasOfLevel[Random.Range(0, wpDatasOfLevel.Length)];
        currentLevel++;
        return currentWpData;
    }
}
