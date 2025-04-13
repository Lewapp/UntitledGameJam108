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
    public float difficultyMultiplier;
    public Vector2 specialSpawnRate;
    public Vector2 hordeSpawnRate;

    private IUiReadable difficultyManager;
    private DifficultyInfo currentDifficulty;
    private int spawners = 0;
    private float nextSpecialTime;
    private float nextHordeTime;
    private float timeSurvived = 0f;

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

        if (currentDifficulty.difficultyType != DifficultyInfo.Difficulties.Hard)
        {
            nextHordeTime = hordeSpawnRate.x;
        }
    }

    private void Update()
    {
        if (currentDifficulty == null)
            return;

        timeSurvived += Time.deltaTime;
        GlobalData.playerInfo.timeSurvived = timeSurvived;

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

        UniqueSpawn();
    }

    private void UniqueSpawn()
    {
        Vector2 minMax;
        float multiplier = timeSurvived * difficultyMultiplier;

        nextSpecialTime -= Time.deltaTime;
        if (nextSpecialTime <= 0)
        {
            minMax = specialSpawnRate;
            minMax.x = minMax.x * multiplier;
            minMax.y = minMax.y * multiplier;

            nextSpecialTime = Mathf.Clamp(Random.Range(specialSpawnRate.x - minMax.x, specialSpawnRate.y - minMax.y), 0, specialSpawnRate.y);
            AttemptAtLockedSpawners(Spawner.SpawnerType.Special, currentDifficulty.specialSpawnerInfo);
        }

        nextHordeTime -= Time.deltaTime;
        if (nextHordeTime <= 0)
        {
            minMax = hordeSpawnRate;
            minMax.x = minMax.x * multiplier;
            minMax.y = minMax.y * multiplier;

            nextHordeTime = Mathf.Clamp(Random.Range(hordeSpawnRate.x - minMax.x, hordeSpawnRate.y - minMax.y), 0, hordeSpawnRate.y);
            AttemptAtLockedSpawners(Spawner.SpawnerType.Horde, currentDifficulty.hordeSpawnerInfo);
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

    private void AttemptAtLockedSpawners(Spawner.SpawnerType chosenType, SpawnerInfo spawnerInfo)
    {
        bool success = true;

        List<ISpawnerable> specialSpawners = new List<ISpawnerable>();
        foreach (GameObject go in spawnDirectables)
        {
            ISpawnerable spawnerable = go.GetComponent<ISpawnerable>();
            if (spawnerable == null)
                continue;

            if (spawnerable.GetSpawnType() == chosenType)
                specialSpawners.Add(spawnerable);
        }

        int index = 0;
        for (int i = 0; i < spawnerInfo.stats.Length; i++)
        {
            if (spawnerInfo.stats[i].difficulty == currentDifficulty.difficultyType)
            {
                index = i;
                break;
            }
        }

        spawnerInfo.locked = false;
        List<ISpawnerable> usedSpawners = new List<ISpawnerable>();
        float chance = 0f;
        while (success)
        {
            success = false;
            if (spawnerInfo.stats[index].spawnChance <= 0)
                break;

            chance = Random.Range(0f, 1f);
            foreach (ISpawnerable spawner in specialSpawners)
            {
                if (usedSpawners.Contains(spawner))
                    continue;

                if (chance <= spawnerInfo.stats[index].spawnChance)
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
