using System.Collections.Generic;

public class InfoStore
{
    #region Properties and References
    // Stores the actual data values for each info type
    private Dictionary<InfoType, object> infoMap = new();

    // Stores whether each info type is currently locked
    private Dictionary<InfoType, bool> infoLocks = new();
    #endregion

    /// <summary>
    /// Assigns a value to a specific info type.
    /// </summary>
    public void SetInfo<T>(InfoType type, T value)
    {
        infoMap[type] = value;
    }

    /// <summary>
    /// Sets the lock state (on or off) for multiple info types at once.
    /// </summary>
    public void SetInfoLocks(InfoType[] types, bool set)
    {
        foreach (InfoType type in types)
        {
            infoLocks[type] = set;
        }
    }

    /// <summary>
    /// Sets the lock state for a single info type.
    /// </summary>
    public void SetInfoLock(InfoType type, bool set)
    {
        infoLocks[type] = set;
    }

    /// <summary>
    /// Tries to retrieve the value for a given info type, if available and castable to the expected type.
    /// </summary>
    public bool TryGetInfo<T>(InfoType type, out T value)
    {
        if (infoMap.TryGetValue(type, out object rawValue) && rawValue is T casted)
        {
            value = casted;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Returns whether a specific info type is currently locked for reading.
    /// </summary>
    public bool CheckInfoLock(InfoType type)
    {
        if (!infoLocks.ContainsKey(type))
        {
            return false;
        }

        return infoLocks[type];
    }

    /// <summary>
    /// Enum representing the types of UI-related information that can be stored and tracked.
    /// </summary>
    public enum InfoType
    {
        None,        // Placeholder / default
        Health,      // Player's health
        Shield,      // Player's shield
        Dashes,      // Number of available dashes
        Difficulty,  // Difficulty Level
        Spawner,     // Spawner Info
    }
}