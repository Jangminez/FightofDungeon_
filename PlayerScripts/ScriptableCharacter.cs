using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Character")]
public class ScriptableCharacter : ScriptableObject
{
    public string characterName;
    public Sprite characterImg;
    public float hp;
    public float mp;
    public float attack;
    public float attackSpeed;
    public float defense;
    public float speed;
    public float critical;
}
