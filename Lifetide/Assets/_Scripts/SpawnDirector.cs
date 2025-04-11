using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnDirector : MonoBehaviour
{
    public static readonly List<IDirectable> spawnDirectables = new List<IDirectable>();
    public static List<GameObject> spawnedEnemies = new List<GameObject>();
    public static List<GameObject> spawnedEnemiesMemory = new List<GameObject>();

    public GameObject player;

    private IUiReadable difficultyManager;
    private int spawners = 0;
    public int enemies = 0;

    private void Awake()
    {
        difficultyManager = GetComponent<IUiReadable>();
    }

    private void Update()
    {
        if (spawners < spawnDirectables.Count)
        {
            ApplyDifficultyToSpawner(spawnDirectables[spawners]);
            spawners++;
        }

        if (!spawnedEnemiesMemory.SequenceEqual(spawnedEnemies))
        {
            spawnedEnemiesMemory = new List<GameObject>();
            foreach (GameObject go in spawnedEnemies)
            {
                spawnedEnemiesMemory.Add(go);
                foreach (IDirectable directable in go.GetComponents<IDirectable>())
                {
                    if (!directable.difficultyApplied)
                        ApplyDifficultyToSpawner(directable);
                }
            }
            
            enemies = spawnedEnemies.Count;
        }
    }

    private void ApplyDifficultyToSpawner(IDirectable directee)
    {
        InfoStore infoStore = difficultyManager.GetInfo();
        infoStore.TryGetInfo(InfoStore.InfoType.Difficulty, out DifficultyInfo spawnInfo);
        directee.SetDifficulty(spawnInfo);
    }

    public static void Register(IDirectable directable, GameObject source)
    {
        if (!spawnDirectables.Contains(directable) && source.CompareTag("Spawner"))
        {
            spawnDirectables.Add(directable);
        }
        else if (!spawnedEnemies.Contains(source))
        {
            spawnedEnemies.Add(source);
        }

        Debug.Log(source.name + " clocking in");
    }

    public static void UnRegister(IDirectable directable, GameObject source)
    {
        if (source.CompareTag("Spawner"))
        {
            spawnDirectables.Remove(directable);
        }
        else
        {
            spawnedEnemies.Remove(source);
        }

        Debug.Log(source.name + " clocking out");
    }
}
