using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Buff", fileName = "new buff")]
public class BuffData : ScriptableObject
{
    public Sprite buffSprite;
    public BuffType buffType;
    public string buffName;
    public string description;
}


public enum BuffType
{
    White,      //for player stat
    Blue,      //for weapon stat
    Yellow,     //for player unique buff
    Red         //for weapon unique buff
}