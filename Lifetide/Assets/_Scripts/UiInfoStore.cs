using System.Collections.Generic;
using UnityEngine;

public class UiInfoStore
{
    private Dictionary<UiInfoType, object> infoMap = new();
    private Dictionary<UiInfoType, bool> infoLocks = new();

    public void SetInfo<T>(UiInfoType type, T value)
    {
        infoMap[type] = value;
    }

    public void SetInfoLocks(UiInfoType[] types, bool set)
    {
        foreach (UiInfoType type in types)
        {
            infoLocks[type] = set;
        }
    }

    public void SetInfoLock(UiInfoType type, bool set)
    {
        infoLocks[type] = set;
    }

    public bool TryGetInfo<T>(UiInfoType type, out T value)
    {
        if (infoMap.TryGetValue(type, out object rawValue) && rawValue is T casted)
        {
            value = casted;
            return true;
        }

        value = default;
        return false;
    }

    public bool CheckInfoLock(UiInfoType type)
    {
        if (!infoLocks.ContainsKey(type))
        {
            return false;
        }

        return infoLocks[type];
    }

    public enum UiInfoType
    {
        None,
        Health,
        Shield,
        Dashes
    }
}