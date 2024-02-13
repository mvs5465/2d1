using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Frostbolt : MonoBehaviour
{
    public FrostboltData frostboltData;
    public Vector3 target;
    private SpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider2D;
    private Light2D boltLight;

    public static Frostbolt Throw(FrostboltData frostboltData, Vector3 start, Vector3 target)
    {
        GameObject container = new("Frostbolt");
        Frostbolt frostbolt = container.AddComponent<Frostbolt>();
        frostbolt.frostboltData = frostboltData;
        frostbolt.transform.position = start;
        frostbolt.target = target;
        return frostbolt;
    }

    private void Start()
    {
        gameObject.layer = frostboltData.layer;
        spriteAnimator = SpriteAnimator.Build(gameObject, frostboltData.boltAnimation);
        circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = frostboltData.gravityScale;

        Vector3 targetDirection = target - transform.localPosition;
        targetDirection /= targetDirection.magnitude;
        targetDirection *= frostboltData.speed;
        rb.AddForce(targetDirection, ForceMode2D.Impulse);

        boltLight = gameObject.AddComponent<Light2D>();
        boltLight.lightType = Light2D.LightType.Point;
        boltLight.intensity = frostboltData.lightIntensity;
        boltLight.pointLightInnerRadius = frostboltData.lightInnerRadius; ;
        boltLight.pointLightOuterRadius = frostboltData.lightOuterRadius;
        boltLight.falloffIntensity = frostboltData.lightFalloffIntensity;
        boltLight.color = Color.white / 2 + Color.blue;

        Invoke(nameof(End), frostboltData.range);
        InvokeRepeating(nameof(RandomizeLight), 0f, 0.15f);
    }

    private void RandomizeLight()
    {
        boltLight.intensity = UnityEngine.Random.Range(frostboltData.lightIntensity / 3, frostboltData.lightIntensity);
        float whiteBias = UnityEngine.Random.Range(0f, 1f);
        Color whiteLevel = Color.white * whiteBias;
        Color blueLevel = Color.blue * (1 - whiteBias);
        boltLight.color = whiteLevel + blueLevel;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity)
        {
            entity.Damage(frostboltData.damage);
        }

        circleCollider2D.isTrigger = true;
        circleCollider2D.radius *= 2;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteAnimator.SetAnimation(frostboltData.explosionAnimation);

        CancelInvoke(nameof(End));
        Invoke(nameof(Pulse), 0);
        Invoke(nameof(End), frostboltData.explosionAnimation.GetDuration());
    }

    private void Pulse()
    {
        circleCollider2D.enabled = !circleCollider2D.enabled;
        Invoke(nameof(Pulse), 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Browner browner = other.gameObject.GetComponent<Browner>();
        if (browner)
        {
            browner.Damage(1);
        }
    }

    private void End()
    {
        Destroy(gameObject);
    }
}