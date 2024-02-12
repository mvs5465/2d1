using UnityEngine;

public class ThrowingStar : MonoBehaviour
{
    public GameConfig gameConfig;
    public Vector3 targetLocation;

    public static ThrowingStar Throw(GameObject parent, GameConfig gameConfig, Vector3 targetLocation)
    {
        GameObject starContainer = Utility.AttachChildObject(parent, "ThrowingStar");
        starContainer.transform.parent = null;
        ThrowingStar throwingStar = starContainer.AddComponent<ThrowingStar>();
        throwingStar.gameConfig = gameConfig;
        throwingStar.targetLocation = targetLocation;
        return throwingStar;
    }

    private void Start()
    {
        SpriteAnimator.Build(gameObject, gameConfig.throwingStarAnimation);

        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0.5f;

        Vector2 launchVector = targetLocation - transform.position;
        launchVector /= launchVector.magnitude;
        launchVector *= 10;
        rb.AddForce(launchVector, ForceMode2D.Impulse);

        Invoke(nameof(Expire), 2);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Browner>())
        {
            other.GetComponent<Browner>().Damage(1);
            Expire();
        }
    }

    private void Expire()
    {
        Destroy(gameObject);
    }
}