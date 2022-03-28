using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoomNode
{
    public bool left, right, up, down;
    public RoomType roomType;
    public RoomNode leftRN, rightRN, downRN, upRN;
}

public enum RoomType
{
    RootRoom,
    NormalRoom,
    BossRoom,
    MysticRoom,
    SupplyRoom,
    RewardRoom
}