using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/Player_Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Properties")]
    public int playerStr = 11;
    public int playerVit = 11;
    public float playerAgi = 3f;
    public int EXPBounty = 75;

    [Header("Player Status Converter")]
    public int statusConverter = 10;
}