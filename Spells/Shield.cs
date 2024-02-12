using UnityEngine;

public class Shield : MonoBehaviour
{
    public ShieldData shieldData;
    private SpriteAnimator spriteAnimator;
    private CircleCollider2D shieldCollider;
    private bool coolingDown = false;

    public static Shield Build(GameObject parent, ShieldData shieldData)
    {
        GameObject shieldContainer = Utility.AttachChildObject(parent, "Shield");
        Shield shield = shieldContainer.AddComponent<Shield>();
        shield.shieldData = shieldData;
        return shield;
    }

    private void Start()
    {
        gameObject.layer = shieldData.layer;
        transform.localScale = shieldData.scale * transform.parent.localScale;

        spriteAnimator = SpriteAnimator.Build(gameObject, shieldData.shieldIdle);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);

        shieldCollider = gameObject.AddComponent<CircleCollider2D>();
        shieldCollider.isTrigger = true;

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        shieldCollider.enabled = false;
    }

    private void Update()
    {
        if (!coolingDown && Input.GetKeyDown(KeyCode.Q))
        {
            coolingDown = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            shieldCollider.enabled = true;
            Invoke(nameof(Disable), shieldData.duration);
            Invoke(nameof(EndCooldown), shieldData.cooldown);
        }
    }

    private void Disable()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        shieldCollider.enabled = false;
    }

    private void EndCooldown()
    {
        coolingDown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Browner browner = other.GetComponent<Browner>();
        if (browner)
        {
            if (spriteAnimator.GetAnimation() != shieldData.shieldImpact)
            {
                spriteAnimator.SetAnimation(shieldData.shieldImpact);
                Invoke(nameof(IdleAnimation), spriteAnimator.GetDuration());
            }
            browner.Damage(1);
            Vector2 deflectionDirection = -(transform.position - other.transform.position);
            deflectionDirection /= deflectionDirection.magnitude;
            browner.GetComponent<Rigidbody2D>().AddForce(deflectionDirection * 10, ForceMode2D.Impulse);
        }
    }

    private void IdleAnimation()
    {
        spriteAnimator.SetAnimation(shieldData.shieldIdle);
    }
}