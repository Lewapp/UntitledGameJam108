using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawner Info", menuName = "ScriptableObjects/SpawnerInfo")]
public class SpawnerInfo : ScriptableObject
{
    public bool locked;
    public SpawnerDifficultyStats[] stats;

    [Serializable]
    public class SpawnerDifficultyStats
    {
        public DifficultyInfo.Difficulties difficulty;
        public int selfDestructAmount;
        public int selfLockAmount;
        public SpawnObject[] spawnObjects;
        public Vector2 spawnTimeRange;
        public Vector2Int spawnAmountRange;
        public float timePerMultiSpawn;
        public int maximumSpawns;
        public int spawnsCheckThreshold;
        [Range(0f, 1f)]
        public float spawnChance = 1f;
    }


    [Serializable]
    public class SpawnObject
    {
        public GameObject spawnee;
        public float spawnInfluence;
    }
}
