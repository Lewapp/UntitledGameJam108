using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class Spawner : MonoBehaviour, IDirectable, ISpawnerable
{
    #region Properties and References

    public bool difficultyApplied { get; set; }
    public bool personalLock { get; set; }

    public SpawnerType spawnerType;
    public SpawnerInfo spawnerInfo;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private int spawnsSinceLastCheck = 0;
    private float timeSinceLastSpawn = 0f;
    private float multiSpawnTime = 0f;
    private float nextRandomTime;
    private int totalSpawns;
    private int totalSpawns_Reset;
    private int difficultyIndex;

    [SerializeField]
    private int totalActiveSpawns;

    #endregion

    private void Awake()
    {
        SetRandomSpawnTime();
        SpawnDirector.Register(this, gameObject);
    }

    private void OnDestroy()
    {
        SpawnDirector.UnRegister(this, gameObject);
    }

    private void Update()
    {
        totalActiveSpawns = spawnedObjects.Count;
        SelfLockCheck();
        SelfDestructCheck();

        if (spawnerInfo.locked || personalLock) return;

        if (spawnsSinceLastCheck >= spawnerInfo.stats[difficultyIndex].spawnsCheckThreshold)
        {
            CheckSpawnsStatus();
            spawnsSinceLastCheck = 0;
        }

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= nextRandomTime)
        {
            timeSinceLastSpawn = 0f;
            if (spawnedObjects.Count < spawnerInfo.stats[difficultyIndex].maximumSpawns)
            {
                StartCoroutine(Spawn(UnityEngine.Random.Range(spawnerInfo.stats[difficultyIndex].spawnAmountRange.x, spawnerInfo.stats[difficultyIndex].spawnAmountRange.y)));
            }
            else
            {
                CheckSpawnsStatus();
            }
        }
    }

    private void SelfDestructCheck()
    {
        if (totalSpawns >= spawnerInfo.stats[difficultyIndex].selfDestructAmount && spawnerInfo.stats[difficultyIndex].selfDestructAmount > 0)
        {
            Destroy(gameObject);
        }
    }

    private void SelfLockCheck()
    {
        if (totalSpawns_Reset >= spawnerInfo.stats[difficultyIndex].selfLockAmount && spawnerInfo.stats[difficultyIndex].selfLockAmount > 0)
        {
            personalLock = true;
            totalSpawns_Reset = 0;
        }
    }

    public void SetDifficulty(DifficultyInfo spawnInfo)
    {
        if (!spawnInfo) return;

        switch (spawnerType)
        {
            case SpawnerType.Generic:
                spawnerInfo = spawnInfo.genericSpawnerInfo;
                break;
            case SpawnerType.Special:
                spawnerInfo = spawnInfo.specialSpawnerInfo;
                break;
            case SpawnerType.Boss:
                spawnerInfo = spawnInfo.bossSpawnerInfo;
                break;
        }

        for (int i = 0; i < spawnerInfo.stats.Length; i++)
        {
            if (spawnerInfo.stats[i].difficulty == spawnInfo.difficultyType)
            {
                difficultyIndex = i;
                break;
            }
        }

        difficultyApplied = true;
    }

    private IEnumerator Spawn(int amount)
    {
        if (spawnerInfo.stats[difficultyIndex].spawnObjects.Length > 0)
        {
            GameObject chosenSpawnObject = null;

            float total = 0f;
            foreach (SpawnerInfo.SpawnObject so in spawnerInfo.stats[difficultyIndex].spawnObjects)
            {
                total += so.spawnInfluence;
            }

            float chosenInfluence = UnityEngine.Random.Range(0, total);
            total = 0f;
            foreach (SpawnerInfo.SpawnObject so in spawnerInfo.stats[difficultyIndex].spawnObjects)
            {
                if (total <= chosenInfluence)
                {
                    chosenSpawnObject = so.spawnee;
                }
                total += so.spawnInfluence;
            }

            if (chosenSpawnObject)
            {
                for (int i = 0; i < amount; i++)
                {
                    totalSpawns++;
                    totalSpawns_Reset++;
                    spawnedObjects.Add(Instantiate(chosenSpawnObject, transform.position, Quaternion.identity));
                    yield return new WaitForSeconds(multiSpawnTime);
                }

                spawnsSinceLastCheck++;
            }
        }
    }

    private void CheckSpawnsStatus()
    {
        spawnedObjects = spawnedObjects.Where(spawned => spawned != null).ToList();
    }

    private void SetRandomSpawnTime()
    {
        nextRandomTime = UnityEngine.Random.Range(spawnerInfo.stats[difficultyIndex].spawnTimeRange.x, spawnerInfo.stats[difficultyIndex].spawnTimeRange.y);
    }

    public SpawnerType GetSpawnType()
    {
        return spawnerType;
    }

    public enum SpawnerType
    { 
        None,
        Generic,
        Special,
        Horde,
        Boss,
    }
}
