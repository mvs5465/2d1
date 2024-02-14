using UnityEngine;

public class Quake : MonoBehaviour
{
    private QuakeData quakeData;
    public static Quake Cast(GameObject parent, QuakeData quakeData, Vector2 localOffset)
    {
        GameObject qgo = Utility.AttachChildObject(parent, "Quake");
        qgo.transform.localPosition = localOffset;
        Quake quake = qgo.AddComponent<Quake>();
        quake.quakeData = quakeData;
        return quake;
    }

    private void Start()
    {
        transform.localScale = transform.parent.localScale * 0.5f;
        SpriteAnimator.Build(gameObject, quakeData.animation, "Effect");
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        Destroy(gameObject, quakeData.animation.GetDuration());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Wizard wizard = other.GetComponent<Wizard>();
        if (wizard)
        {
            wizard.Damage(quakeData.damage);
        }
    }
}