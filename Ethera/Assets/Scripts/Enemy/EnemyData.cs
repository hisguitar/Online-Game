using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObject/Enemy_Data", order = 0)]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public int hp;
    public int EXPBounty;
    public int enemyStr;

    [Header("Enemy Patrol Stats")]
    public float patrolSpeed;
    public float patrolDistance;
    public float idleTime;

    [Header("Enemy Chase Stats")]
    public float chaseSpeed;
    public float chaseDistance;
}