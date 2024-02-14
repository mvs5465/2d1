using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/GolemData")]
public class GolemData : ScriptableObject
{
    public SpriteAnimatorData idleAnimation;
    public SpriteAnimatorData walkAnimation;
    public SpriteAnimatorData attackAnimation;
    public SpriteAnimatorData deathAnimation;
    public float aggroRadius = 5;
    public float attackCooldown = 4;
    public float attackRange = 2;
    public float maxSpeed = 2;
    public int layer = 0;
    public QuakeData quakeData;
}