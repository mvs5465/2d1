using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpriteAnimatorData")]
public class SpriteAnimatorData : ScriptableObject
{
    public float frameDuration = 0.1f;
    [SerializeField] public List<Sprite> frames;

    public float GetDuration()
    {
        return frameDuration * frames.Count;
    }
}