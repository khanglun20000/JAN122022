using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DectectPlayerEnterRoom : MonoBehaviour
{
    RoomGenerator roomGenerator;
    RoomController roomController;

    bool canTrigger = true;

    private void Awake()
    {
        roomGenerator = transform.parent.GetComponent<RoomGenerator>();
        roomController = transform.parent.GetComponent<RoomController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTrigger)
        {
            canTrigger = false;
            roomGenerator.ActivateMapRep();
            roomController.RoomTriggered();
        }
        else if(collision.CompareTag("Player") && !canTrigger)
        {
            roomGenerator.ActivateMapRep();
        }
    }

    public void SetCanTrigger(bool value)
    {
        canTrigger = value;
    }
}
