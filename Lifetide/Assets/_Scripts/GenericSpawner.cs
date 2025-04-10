using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class GenericSpawner : MonoBehaviour
{
    #region Properties and References

    public bool locked;
    public SpawnObject[] spawnObjects;
    public Vector2 spawnTimeRange;
    public Vector2Int spawnAmountRange;
    public float timePerMultiSpawn;
    public int maximumSpawns;
    public int spawnsCheckThreshold;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private int spawnsSinceLastCheck = 0;
    private float timeSinceLastSpawn = 0f;
    private float multiSpawnTime = 0f;
    private float nextRandomTime;

    [SerializeField]
    private int totalSpawns;

    #endregion

    private void Awake()
    {
        SetRandomSpawnTime();
    }

    private void Update()
    {
        totalSpawns = spawnedObjects.Count;

        if (spawnsSinceLastCheck >= spawnsCheckThreshold)
        {
            CheckSpawnsStatus();
            spawnsSinceLastCheck = 0;
        }

        if (locked) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= nextRandomTime)
        {
            timeSinceLastSpawn = 0f;
            if (spawnedObjects.Count < maximumSpawns)
            {
                StartCoroutine(Spawn(UnityEngine.Random.Range(spawnAmountRange.x, spawnAmountRange.y)));
            }
            else
            {
                CheckSpawnsStatus();
            }
        }
    }

    private IEnumerator Spawn(int amount)
    {
        if (spawnObjects.Length > 0)
        {
            GameObject chosenSpawnObject = null;

            float total = 0f;
            foreach (SpawnObject so in spawnObjects)
            {
                total += so.spawnInfluence;
            }

            float chosenInfluence = UnityEngine.Random.Range(0, total);
            total = 0f;
            foreach (SpawnObject so in spawnObjects)
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
        nextRandomTime = UnityEngine.Random.Range(spawnTimeRange.x, spawnTimeRange.y);
    }

    [Serializable]
    public class SpawnObject
    {
        public GameObject spawnee;
        public float spawnInfluence;
    }
}
