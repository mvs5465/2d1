using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ArcaneOrb : MonoBehaviour
{
    public ArcaneOrbData arcaneOrbData;
    public Vector3 target;
    private SpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider2D;
    private Light2D boltLight;

    public static ArcaneOrb Throw(ArcaneOrbData arcaneOrbData, Vector3 start, Vector3 target)
    {
        GameObject container = new("ArcaneOrb");
        ArcaneOrb arcaneOrb = container.AddComponent<ArcaneOrb>();
        arcaneOrb.arcaneOrbData = arcaneOrbData;
        arcaneOrb.transform.position = start;
        arcaneOrb.target = target;
        return arcaneOrb;
    }

    private void Start()
    {
        gameObject.layer = arcaneOrbData.layer;
        spriteAnimator = SpriteAnimator.Build(gameObject, arcaneOrbData.boltAnimation, "Effect");
        circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = arcaneOrbData.gravityScale;

        Vector3 targetDirection = target - transform.localPosition;
        targetDirection /= targetDirection.magnitude;
        targetDirection *= arcaneOrbData.speed;
        rb.AddForce(targetDirection, ForceMode2D.Impulse);

        boltLight = gameObject.AddComponent<Light2D>();
        boltLight.lightType = Light2D.LightType.Point;
        boltLight.intensity = arcaneOrbData.lightIntensity;
        boltLight.pointLightInnerRadius = arcaneOrbData.lightInnerRadius;
        boltLight.pointLightOuterRadius = arcaneOrbData.lightOuterRadius;
        boltLight.falloffIntensity = arcaneOrbData.lightFalloffIntensity;
        boltLight.color = new Color(194, 123, 172);

        HomingDetector.Build(gameObject, arcaneOrbData.homingRadius);

        Invoke(nameof(End), arcaneOrbData.range);
        InvokeRepeating(nameof(RandomizeLight), 0f, 0.15f);
    }

    private void RandomizeLight()
    {
        boltLight.intensity = Random.Range(arcaneOrbData.lightIntensity / 3, arcaneOrbData.lightIntensity);
        float whiteBias = Random.Range(0f, 1f);
        Color whiteLevel = Color.white * whiteBias;
        Color purpleLevel = new Color(194, 123, 172) * (1 - whiteBias);
        boltLight.color = whiteLevel + purpleLevel;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity)
        {
            entity.Damage(arcaneOrbData.damage);
        }

        circleCollider2D.isTrigger = true;
        circleCollider2D.radius *= 2;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteAnimator.SetAnimation(arcaneOrbData.explosionAnimation);

        CancelInvoke(nameof(End));
        Invoke(nameof(Pulse), 0);
        Invoke(nameof(End), arcaneOrbData.explosionAnimation.GetDuration());
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