using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty Info", menuName = "ScriptableObjects/DifficultyInfo")]
public class DifficultyInfo : ScriptableObject
{
    public Difficulties difficultyType;
    public float stunDuration;

    [Header("Characters")]
    public List<CharacterStats> characterStats;

    [Header("Spawners")]
    public float specialEnemyChance;
    public float bossChance;
    public SpawnerInfo genericSpawnerInfo;
    public SpawnerInfo specialSpawnerInfo;
    public SpawnerInfo hordeSpawnerInfo;
    public SpawnerInfo bossSpawnerInfo;

    public enum Difficulties
    {
        Easy,
        Medium,
        Hard,
    }

}
