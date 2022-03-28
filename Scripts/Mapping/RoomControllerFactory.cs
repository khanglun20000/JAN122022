using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

public abstract class RoomController : MonoBehaviour
{
    public delegate void OnRoomTrigger();
    public OnRoomTrigger RoomTriggered;
    public delegate void OnRoomDeactivated();
    public OnRoomTrigger RoomDeactivated;
    public delegate void OnClearRoom();
    public OnClearRoom RoomCleared;

    protected bool isRoomCleared;
    protected RoomGenerator RG;
    protected Transform myTransform;

    [HideInInspector] public List<Transform> Detects;
    [HideInInspector] public List<Transform> Doors;
    public Transform TilesContainer;

    public abstract RoomType RoomType { get; }
    public abstract void StartRoomEvent();
    public abstract void ClearRoom();
    protected virtual void Start()
    {
        RG = GetComponent<RoomGenerator>();
        myTransform = transform;

        RoomTriggered += TriggerRoom;
        RoomDeactivated += DeactivateRoom;
        RoomCleared += ClearRoom;

        Detects = RG.detects;
        Doors = RG.doors;
    }

    public bool GetIsRoomCleared()
    {
        return isRoomCleared;
    }

    void TriggerRoom()
    {
        foreach (Transform _Detect in Detects)
        {
            _Detect.GetComponent<DectectPlayerEnterRoom>().SetCanTrigger(false);
        }
        foreach (Transform _Door in Doors)
        {
            _Door.gameObject.SetActive(true);
        }
        StartRoomEvent();
        DungeonGenerator.instance.RoomPlayerIn = myTransform;
        PlayerStatController.instance.IsBattling = true;
    }

    void DeactivateRoom()
    {
        foreach (Transform _Door in Doors)
        {
            Destroy(_Door.gameObject);
        }
        PlayerStatController.instance.IsBattling = false;
    }
}

public static class RoomConTrollerFactory
{
    static Dictionary<RoomType, Type> typeNames;

    static RoomConTrollerFactory()
    {
        var typeTypes = Assembly.GetAssembly(typeof(RoomController)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract
            && myType.IsSubclassOf(typeof(RoomController)));

        typeNames = new Dictionary<RoomType, Type>();

        foreach (var type in typeTypes)
        {
            RoomController tempType = FactoriesProductsContainer.instance.gameObject.AddComponent(type) as RoomController;
            tempType.enabled = false;
            typeNames.Add(tempType.RoomType, type);
        }
    }

    public static RoomController GetRoomType(RoomType _roomType, GameObject _gameObject)
    {
        if (typeNames.ContainsKey(_roomType))
        {
            Type type = typeNames[_roomType];
            var mysticEvent = _gameObject.AddComponent(type) as RoomController;
            return mysticEvent;
        }
        return null;
    }
}