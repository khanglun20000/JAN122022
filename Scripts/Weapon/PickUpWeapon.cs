using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    public WeaponData wpData;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = wpData.wpImage;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<UpdateHoldWeapon>().pickedUp(wpData);
            DungeonGenerator.instance.RoomPlayerIn.GetComponent<RoomController>().RoomDeactivated();
        }
    }
}
