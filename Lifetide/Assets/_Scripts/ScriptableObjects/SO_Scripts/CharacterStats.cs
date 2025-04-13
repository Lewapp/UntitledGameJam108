using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public characterTypes characterType; // Name of character
    public CharacterDifficultyStats[] stats;

    public enum characterTypes
    {
        None,
        Player,
        Basic,
        Dualer,
        Charger,
        Healer,
        Jumper, 
        Bomber,
    }

    [Serializable]
    public class CharacterDifficultyStats
    {
        public DifficultyInfo.Difficulties difficulty;
        public float maxHealth; // Max health of character
        public float mass; // Mass of Character
        public float moveSpeed;
        public float attackMoveSpeed;
        public float damageMp;
        public float penetrationMp;
        public float delayMp;
        public float attackSpeedMp;
    }

}
