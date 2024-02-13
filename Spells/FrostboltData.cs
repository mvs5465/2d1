using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/FrostboltData")]
public class FrostboltData : SpellData
{
    public int damage = 1;
    public float range = 1f;
    public float speed = 5f;
    public int layer = 0;

    public SpriteAnimatorData boltAnimation;
    public SpriteAnimatorData explosionAnimation;
    public float gravityScale = 0.5f;
    public float lightIntensity = 5;
    public float lightInnerRadius = 3;
    public float lightOuterRadius = 4;
    public float lightFalloffIntensity = 1;

    public override void Cast(Vector3 startPos, Vector3 targetPos)
    {
        Frostbolt.Throw(this, startPos, targetPos);
    }

    public override SpriteAnimatorData GetSpellbookAnimation()
    {
        return boltAnimation;
    }
}