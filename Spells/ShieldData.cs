using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/ShieldData")]
public class ShieldData : SpellData
{
    public SpriteAnimatorData shieldIdle;
    public SpriteAnimatorData shieldImpact;
    public int layer;
    public float scale = 2;
    public float duration = 2;
    public float cooldown = 5;

    public override void Cast(Vector3 startPos, Vector3 targetPos)
    {
        return;
    }

    public override SpriteAnimatorData GetSpellbookAnimation()
    {
        return shieldIdle;
    }

    public override void Pickup()
    {
        base.Pickup();
        Shield.Build(FindObjectOfType<Wizard>().gameObject, this);
    }
}