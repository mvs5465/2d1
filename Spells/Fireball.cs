using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fireball : MonoBehaviour
{
    public FireballData fireballData;
    public Vector3 target;
    private SpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider2D;
    private Light2D boltLight;

    public static Fireball Throw(FireballData fireballData, Vector3 start, Vector3 target)
    {
        GameObject container = new("Fireball");
        Fireball fireball = container.AddComponent<Fireball>();
        fireball.fireballData = fireballData;
        fireball.transform.position = start;
        fireball.target = target;
        return fireball;
    }

    private void Start()
    {
        gameObject.transform.localScale *= fireballData.spellDisplaySize;
        gameObject.layer = fireballData.layer;
        spriteAnimator = SpriteAnimator.Build(gameObject, fireballData.boltAnimation);
        circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = fireballData.gravityScale;

        Vector3 targetDirection = target - transform.localPosition;
        targetDirection /= targetDirection.magnitude;
        targetDirection *= fireballData.speed;
        rb.AddForce(targetDirection, ForceMode2D.Impulse);

        boltLight = gameObject.AddComponent<Light2D>();
        boltLight.lightType = Light2D.LightType.Point;
        boltLight.intensity = fireballData.lightIntensity;
        boltLight.pointLightInnerRadius = fireballData.lightInnerRadius; ;
        boltLight.pointLightOuterRadius = fireballData.lightOuterRadius;
        boltLight.falloffIntensity = fireballData.lightFalloffIntensity;
        boltLight.color = Color.white / 2 + Color.red;

        Invoke(nameof(End), fireballData.range);
        InvokeRepeating(nameof(RandomizeLight), 0f, 0.15f);
    }

    private void RandomizeLight()
    {
        boltLight.intensity = Random.Range(fireballData.lightIntensity / 3, fireballData.lightIntensity);
        float whiteBias = Random.Range(0f, 1f);
        Color whiteLevel = Color.white * whiteBias;
        Color redLevel = Color.red * (1 - whiteBias);
        boltLight.color = whiteLevel + redLevel;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity)
        {
            entity.Damage(fireballData.damage);
        }
        Wizard wizard = other.gameObject.GetComponent<Wizard>();
        if (wizard)
        {
            wizard.Damage(fireballData.pulseDamage);
        }

        circleCollider2D.isTrigger = true;
        circleCollider2D.radius *= 2;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteAnimator.SetAnimation(fireballData.explosionAnimation);

        CancelInvoke(nameof(End));
        Invoke(nameof(Pulse), 0);
        Invoke(nameof(End), fireballData.explosionAnimation.GetDuration());
    }

    private void Pulse()
    {
        circleCollider2D.enabled = !circleCollider2D.enabled;
        Invoke(nameof(Pulse), 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity)
        {
            entity.Damage(fireballData.pulseDamage);
        }
        Wizard wizard = other.gameObject.GetComponent<Wizard>();
        if (wizard)
        {
            wizard.Damage(fireballData.pulseDamage);
        }
    }

    private void End()
    {
        Destroy(gameObject);
    }
}