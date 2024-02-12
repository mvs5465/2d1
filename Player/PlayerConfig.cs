using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public int layer = 0;
    public int startHealth = 1;
    public int maxHealth = 1;
    public int maxSpeed = 1;
    public float jumpHeight = 1;
    public float sizeMultiplier = 1;
    public SpriteAnimatorData idleAnimation;
    public SpriteAnimatorData runAnimation;
    public SpriteAnimatorData attackAnimation;
}