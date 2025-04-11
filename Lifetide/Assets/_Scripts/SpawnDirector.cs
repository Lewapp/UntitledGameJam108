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

    private IUiReadable difficultyManager;
    private DifficultyInfo currentDifficulty;
    private int spawners = 0;
    public int enemies = 0;

    private void Awake()
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
    }

    private void Start()
    {
        LockNonGenericSpawners();
    }

    private void Update()
    {
        if (spawners < spawnDirectables.Count)
        {
            foreach (IDirectable directable in spawnDirectables[spawners].GetComponents<IDirectable>())
            {
                ApplyDifficultyToSpawner(directable);
            }
            spawners++;
        }

        spawnedEnemiesMemory = CompareLists(spawnedEnemies, spawnedEnemiesMemory, out int eCount);
        enemies = eCount;
        spawnedWeaponsMemory = CompareLists(spawnedWeapons, spawnedWeaponsMemory, out int wCount);
    }

    private void LockNonGenericSpawners()
    {
        foreach (GameObject go in spawnDirectables)
        {
            ISpawnerable spawnerable = go.GetComponent<ISpawnerable>();
            if (spawnerable == null)
                continue;

            if (spawnerable.GetSpawnType() != Spawner.SpawnerType.Generic)
            {
                spawnerable.personalLock = true;
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
