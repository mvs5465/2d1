using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/WhelpData")]
public class WhelpData : ScriptableObject
{
    public SpriteAnimatorData idleAnimation;
    public SpriteAnimatorData attackAnimation;
    public float aggroRadius = 5;
    public float fireballCooldown = 3;
    public int layer = 0;
}