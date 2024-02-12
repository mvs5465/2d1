
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private SpriteAnimatorData animationData;

    private int currentFrame = 0;
    private float timer = 0f;


    public static SpriteAnimator Build(GameObject parent, SpriteAnimatorData animationData)
    {
        return Build(parent, animationData, "Entities");
    }

    public static SpriteAnimator Build(GameObject parent, SpriteAnimatorData animationData, string sortingLayerName)
    {
        return Build(parent, animationData, sortingLayerName, 0);
    }

    public static SpriteAnimator Build(GameObject parent, SpriteAnimatorData animationData, string sortingLayerName, int sortingLayerOrder)
    {
        GameObject controllerObject = new("PseudoAnimationController");
        controllerObject.transform.position = parent.transform.position;
        controllerObject.transform.SetParent(parent.transform);
        SpriteAnimator controller = controllerObject.AddComponent<SpriteAnimator>();
        controller.animationData = animationData;

        if (!parent.GetComponent<SpriteRenderer>()) { parent.AddComponent<SpriteRenderer>(); }
        SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = sortingLayerOrder;
        controller.spriteRenderer = sr;
        controller.SetAnimation(animationData);

        return controller;
    }

    public void SetAnimation(SpriteAnimatorData newSpriteAnimatorData)
    {
        animationData = newSpriteAnimatorData;
        currentFrame = 0;
        timer = 0f;
        if (animationData.frames.Count > 0)
        {
            spriteRenderer.sprite = animationData.frames[0];
        }
    }

    public SpriteAnimatorData GetAnimation()
    {
        return animationData;
    }

    public float GetDuration()
    {
        return animationData.frameDuration * animationData.frames.Count;
    }

    public void Update()
    {
        if (animationData.frames.Count <= 1)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= animationData.frameDuration)
        {
            timer = 0;
            currentFrame = (currentFrame + 1) % animationData.frames.Count;
            spriteRenderer.sprite = animationData.frames[currentFrame];
        }
    }
}