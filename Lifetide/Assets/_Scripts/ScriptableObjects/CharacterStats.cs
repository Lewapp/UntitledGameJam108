using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public string characterName; // Name of character
    public float maxHealth; // Max health of character
    public float mass; // Mass of Character
}
