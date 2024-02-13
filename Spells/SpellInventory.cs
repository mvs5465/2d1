using System.Collections.Generic;
using UnityEngine;

public class SpellInventory : MonoBehaviour
{
    private List<SpellData> spells;
    private SpellData currentSpell;
    private int currentSpellIndex = 0;

    public static SpellInventory Build(GameObject parent)
    {
        GameObject go = Utility.AttachChildObject(parent, "SpellInventory");
        SpellInventory si = go.AddComponent<SpellInventory>();
        return si;
    }

    private void Start()
    {
        spells = new();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && spells.Count >= 2)
        {
            if (currentSpellIndex + 1 >= spells.Count)
            {
                currentSpellIndex = 0;
            }
            else
            {
                currentSpellIndex += 1;
            }
            currentSpell = spells[currentSpellIndex];
            Debug.Log("Switched spells to " + currentSpell.spellName);
        }
    }

    public void Cast(Vector3 startPos, Vector3 targetPos)
    {
        currentSpell.Cast(startPos, targetPos);
    }

    public void AddSpell(SpellData spellData)
    {
        spells.Add(spellData);
        if (!currentSpell) currentSpell = spells[0];
    }



}