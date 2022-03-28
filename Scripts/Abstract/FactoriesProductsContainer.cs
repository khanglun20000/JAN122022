using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoriesProductsContainer : MonoBehaviour
{
    public static FactoriesProductsContainer instance;
    public List<RoomController> roomControllerTypes = new List<RoomController>();

    private void Awake()
    {
        instance = this;
    }
}
