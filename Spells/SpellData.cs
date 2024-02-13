using UnityEngine;

public abstract class SpellData : ScriptableObject
{
    public string spellName;
    public float spellDisplaySize = 0.75f;

    private bool pickedUp = false;

    public virtual void Pickup()
    {
        if (pickedUp) return;
        pickedUp = true;
        FindObjectOfType<Wizard>().gameObject.GetComponentInChildren<SpellInventory>().AddSpell(this);
        Debug.Log(string.Format("Picked up {0}", spellName));
    }
    public abstract SpriteAnimatorData GetSpellbookAnimation();
    public abstract void Cast(Vector3 startPos, Vector3 targetPos);
}