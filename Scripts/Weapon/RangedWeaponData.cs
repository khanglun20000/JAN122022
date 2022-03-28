using UnityEngine;

[CreateAssetMenu(fileName = "new ranged weapon", menuName = "New ranged weapon")]
public class RangedWeaponData : WeaponData
{
    public BulletTypes bulletType;
    public int bulletSpeed;
    public float bulletWidth;
    public bool hasTrail;
    public float bulletLength;
}

