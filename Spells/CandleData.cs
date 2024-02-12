using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/CandleData")]
public class CandleData : SpellData
{
    public SpriteAnimatorData candleOff;
    public SpriteAnimatorData candleStart;
    public SpriteAnimatorData candleLit;
    public int layer;

    public override SpriteAnimatorData GetSpellbookAnimation()
    {
        return candleOff;
    }

    public override void Pickup()
    {
        base.Pickup();
        Candle.Build(FindObjectOfType<Wizard>().gameObject, this);
    }
}