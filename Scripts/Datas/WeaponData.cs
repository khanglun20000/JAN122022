using UnityEngine;

public class WeaponData : ScriptableObject
{
    public Sprite wpImage;
    public int damage;
    public float attackSpeed;
    public CombatStyle combatStyle;
    public RuntimeAnimatorController animator;
}

public enum CombatStyle
{
    bow,
    sword,
    staff
}
