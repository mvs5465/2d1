using UnityEngine;

public abstract class SpellData : ScriptableObject
{
    public string spellName = "";
    public float spellDisplaySize = 0.75f;

    public virtual void Pickup(SpellInventory spellInventory)
    {
        spellInventory.AddSpell(this);
    }
    public abstract SpriteAnimatorData GetSpellbookAnimation();
    public virtual void Cast(Vector3 startPos, Vector3 targetPos) { }
}