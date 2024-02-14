using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Spells/QuakeData")]
public class QuakeData : ScriptableObject
{
    public int damage = 1;
    public SpriteAnimatorData animation;
}