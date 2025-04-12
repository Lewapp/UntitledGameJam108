using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnDirector : MonoBehaviour
{
    public static readonly List<GameObject> spawnDirectables = new List<GameObject>();
    public static List<GameObject> spawnedEnemies = new List<GameObject>();
    public static List<GameObject> spawnedEnemiesMemory = new List<GameObject>();
    public static List<GameObject> spawnedWeapons = new List<GameObject>();
    public static List<GameObject> spawnedWeaponsMemory = new List<GameObject>();

    public GameObject player;
    public Vector2 specialSpawnRate;

    private IUiReadable difficultyManager;
    private DifficultyInfo currentDifficulty;
    private int spawners = 0;
    private float nextSpecialTime;

    private void Start()
    {
        difficultyManager = GetComponent<IUiReadable>();
        if (difficultyManager != null)
        {
            InfoStore infoStore = difficultyManager.GetInfo();
            infoStore.TryGetInfo(InfoStore.InfoType.Difficulty, out DifficultyInfo difficultyInfo);
            currentDifficulty = difficultyInfo;
            currentDifficulty.genericSpawnerInfo.locked = false;
            currentDifficulty.specialSpawnerInfo.locked = true;
            //currentDifficulty.bossSpawnerInfo.locked = true;
        }
        LockNonGenericSpawners();
    }

    private void Update()
    {
        if (currentDifficulty == null)
            return;

        if (spawners < spawnDirectables.Count)
        {
            foreach (IDirectable directable in spawnDirectables[spawners].GetComponents<IDirectable>())
            {
                ApplyDifficultyToSpawner(directable);
            }
            spawners++;
        }

        spawnedEnemiesMemory = CompareLists(spawnedEnemies, spawnedEnemiesMemory, out int eCount);
        spawnedWeaponsMemory = CompareLists(spawnedWeapons, spawnedWeaponsMemory, out int wCount);

        nextSpecialTime -= Time.deltaTime;
        if (nextSpecialTime <= 0)
        {
            nextSpecialTime = Random.Range(specialSpawnRate.x, specialSpawnRate.y);
            AttemptSpecialSpawn();
        }
    }

    private void LockNonGenericSpawners()
    {
        foreach (GameObject go in spawnDirectables)
        {
            ISpawnerable spawnerable = go.GetComponent<ISpawnerable>();
            if (spawnerable == null)
                continue;

            if (spawnerable.GetSpawnType() != Spawner.SpawnerType.Generic)
                spawnerable.personalLock = true;
        }
    }

    private void AttemptSpecialSpawn()
    {
        bool success = true;

        List<ISpawnerable> specialSpawners = new List<ISpawnerable>();
        foreach (GameObject go in spawnDirectables)
        {
            ISpawnerable spawnerable = go.GetComponent<ISpawnerable>();
            if (spawnerable == null)
                continue;

            if (spawnerable.GetSpawnType() == Spawner.SpawnerType.Special)
                specialSpawners.Add(spawnerable);
        }

        int index = 0;
        for (int i = 0; i < currentDifficulty.specialSpawnerInfo.stats.Length; i++)
        {
            if (currentDifficulty.specialSpawnerInfo.stats[i].difficulty == currentDifficulty.difficultyType)
            {
                index = i;
                break;
            }
        }

        currentDifficulty.specialSpawnerInfo.locked = false;
        List<ISpawnerable> usedSpawners = new List<ISpawnerable>();
        float chance = 0f;
        while (success)
        {
            success = false;
            if (currentDifficulty.specialSpawnerInfo.stats[index].spawnChance <= 0)
                break;

            chance = Random.Range(0f, 1f);
            foreach (ISpawnerable spawner in specialSpawners)
            {
                if (usedSpawners.Contains(spawner))
                    continue;

                if (chance <= currentDifficulty.specialSpawnerInfo.stats[index].spawnChance)
                {
                    success = true;
                    usedSpawners.Add(spawner);
                    spawner.personalLock = false; // Allows special spawn

                    break;
                }
            }
        }
    }

    private List<GameObject> CompareLists(List<GameObject> List1, List<GameObject> List2, out int count)
    {
        if (!List2.SequenceEqual(List1))
        {
            List2 = new List<GameObject>();
            foreach (GameObject go in List1)
            {
                List2.Add(go);
                foreach (IDirectable directable in go.GetComponents<IDirectable>())
                {
                    if (!directable.difficultyApplied)
                        ApplyDifficultyToSpawner(directable);
                }
            }
        }

        count = List1.Count;

        return List2;
    }

    private void ApplyDifficultyToSpawner(IDirectable directee)
    {
        InfoStore infoStore = difficultyManager.GetInfo();
        infoStore.TryGetInfo(InfoStore.InfoType.Difficulty, out DifficultyInfo spawnInfo);
        directee.SetDifficulty(spawnInfo);
    }

    public static void Register(IDirectable directable, GameObject source)
    {
        if (!spawnDirectables.Contains(source) && source.CompareTag("Spawner"))
        {
            spawnDirectables.Add(source);
        }
        else if (!spawnedEnemies.Contains(source) && source.CompareTag("Enemy"))
        {
            spawnedEnemies.Add(source);
        }
        else if (!spawnedWeapons.Contains(source))
        {
            spawnedWeapons.Add(source);
        }

        Debug.Log(source.name + " clocking in");
    }

    public static void UnRegister(IDirectable directable, GameObject source)
    {
        spawnDirectables.Remove(source);
        spawnedEnemies.Remove(source);
        spawnedWeapons.Remove(source);

        Debug.Log(source.name + " clocking out");
    }
}
