using UnityEngine;

public abstract class SpellData : ScriptableObject
{
    private bool pickedUp = false;
    public float spellDisplaySize = 0.75f;

    public virtual void Pickup()
    {
        if (pickedUp) return;
        pickedUp = true;
        Debug.Log(string.Format("Picked up {0}", name));
    }
    public abstract SpriteAnimatorData GetSpellbookAnimation();
}