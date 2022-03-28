using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

public enum MysticType
{
    ScrambleBall
}

public abstract class MysticEvent : MonoBehaviour
{
    public abstract MysticType MysticType { get; }
    public abstract void StartMysticEvent();
}


public static class MysticEventFactory
{
    static Dictionary<MysticType, Type> eventNames;

    static MysticEventFactory()
    {
        var eventTypes = Assembly.GetAssembly(typeof(MysticEvent)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract
            && myType.IsSubclassOf(typeof(MysticEvent)));

        eventNames = new Dictionary<MysticType, Type>();

        foreach (var type in eventTypes)
        {
            MysticEvent tempEvent = FactoriesProductsContainer.instance.gameObject.AddComponent(type) as MysticEvent;
            tempEvent.enabled = false;
            eventNames.Add(tempEvent.MysticType, type);
        }
    }

    public static MysticEvent GetMysticEvent(MysticType _mysticType, GameObject _gameObject)
    {
        if (eventNames.ContainsKey(_mysticType))
        {
            Type type = eventNames[_mysticType];
            var mysticEvent = _gameObject.AddComponent(type) as MysticEvent;
            return mysticEvent;
        }
        return null;
    }
}
