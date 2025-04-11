using UnityEngine;
using static Spawner;

public interface ISpawnerable
{
    public bool personalLock { get; set; }
    public SpawnerType GetSpawnType();
}
