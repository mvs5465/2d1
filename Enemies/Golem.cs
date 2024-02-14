using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Golem : Entity
{
    private class GolemAggroManager : MonoBehaviour
    {
        private Golem parentGolem;
        public static GameObject Build(Golem parentGolem, float aggroRadius)
        {
            GameObject gamContainer = Utility.AttachChildObject(parentGolem.gameObject, "GolemAggroManager");
            gamContainer.AddComponent<GolemAggroManager>().parentGolem = parentGolem;
            CircleCollider2D circleCollider2D = gamContainer.AddComponent<CircleCollider2D>();
            circleCollider2D.radius = aggroRadius;
            circleCollider2D.isTrigger = true;
            return gamContainer;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Wizard>())
            {
                parentGolem.NotifyAggroEnter(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Wizard>())
            {
                parentGolem.NotifyAggroExit();
            }
        }
    }

    ///////////////
    /// Begin Class


    public GolemData golemData;
    private SpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private GameObject target;
    private bool coolingDown = false;
    private bool facingRight = false;
    private GameObject gam;
    private Vector2 startPos;
    private bool attacking = false;
    private bool dying = false;
    private float maxSpeed;
    private bool canCastQuake = false;

    public void NotifyAggroEnter(GameObject target)
    {
        this.target = target;
        InvokeRepeating(nameof(ApproachTarget), 1, 0.1f);
        CancelInvoke(nameof(ReturnHome));
    }

    public void NotifyAggroExit()
    {
        target = null;
        InvokeRepeating(nameof(ReturnHome), 1, 0.1f);
        CancelInvoke(nameof(ApproachTarget));
    }

    protected override void StartCall()
    {
        spriteAnimator = SpriteAnimator.Build(gameObject, golemData.idleAnimation);

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 1;
        rb.drag = 0.9f;
        rb.mass *= 10;
        maxSpeed = golemData.maxSpeed;

        // Random chance to spawn a super golem
        if (Random.Range(0, 10) < 2)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red + Color.white / 3;
            rb.mass *= 100;
            maxSpeed *= 1.7f;
            maxHealth *= 2;
            curHealth = maxHealth;
            transform.localScale *= 1.2f;
            canCastQuake = true;

            Light2D superIndicator = gameObject.AddComponent<Light2D>();
            superIndicator.lightType = Light2D.LightType.Point;
            superIndicator.intensity = 0.5f;
            superIndicator.pointLightInnerRadius = 0;
            superIndicator.pointLightOuterRadius = 4;
            superIndicator.color = Color.white / 2 + Color.red;
        }
        gameObject.AddComponent<CapsuleCollider2D>();
        gam = GolemAggroManager.Build(this, golemData.aggroRadius);
        startPos = new Vector2(transform.position.x, transform.position.y);
        gameObject.layer = golemData.layer;
    }

    protected override void Update()
    {
        base.Update();
        if (dying || attacking) return;
        if (!coolingDown && !attacking && target && (target.transform.position - transform.position).magnitude < golemData.attackRange)
        {
            Debug.Log("Golem is attacking!");
            attacking = true;
            coolingDown = true;
            spriteAnimator.SetAnimation(golemData.attackAnimation);
            if (canCastQuake)
            {
                Invoke(nameof(CastQuake), golemData.attackAnimation.GetDuration() / 2);
            }
            Invoke(nameof(EndAttack), golemData.attackAnimation.GetDuration());
            Invoke(nameof(ClearCooldown), golemData.attackCooldown);
            return;
        }
        if (rb.velocity.x < 0 && facingRight)
        {
            facingRight = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (rb.velocity.x > 0 && !facingRight)
        {
            facingRight = true;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (rb.velocity.magnitude > 0.5)
        {
            if (spriteAnimator.GetAnimation() != golemData.walkAnimation)
            {
                spriteAnimator.SetAnimation(golemData.walkAnimation);
            }
        }
        else
        {
            if (spriteAnimator.GetAnimation() != golemData.idleAnimation)
            {
                spriteAnimator.SetAnimation(golemData.idleAnimation);
            }
        }
    }

    private void CastQuake()
    {
        Vector2 offset;
        if (facingRight) offset = Vector2.right;
        else offset = Vector2.left;
        Quake.Cast(gameObject, golemData.quakeData, Vector2.down + offset * 0.75f);
    }

    private void EndAttack()
    {
        Debug.Log("Golem attack ended");
        attacking = false;
        spriteAnimator.SetAnimation(golemData.idleAnimation);
    }

    private void ApproachTarget()
    {
        if (attacking) return;
        if (target.transform.position.x < transform.position.x && rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = (Vector2.left + Vector2.up) * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else if (target.transform.position.x > transform.position.x && rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = (Vector2.right + Vector2.up) * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    private void ReturnHome()
    {
        if (attacking) return;
        if (startPos.x < transform.position.x - 1 && rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = (Vector2.left + Vector2.up) * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else if (startPos.x > transform.position.x + 1 && rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = (Vector2.right + Vector2.up) * rb.mass;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    private void ClearCooldown()
    {
        coolingDown = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Wizard>())
        {
            other.gameObject.GetComponent<Wizard>().Damage(1);
        }
    }

    protected override void Die()
    {
        if (!dying)
        {
            dying = true;
            gam.SetActive(false);
            CancelInvoke();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            gameObject.GetComponent<Collider2D>().enabled = false;
            spriteAnimator.SetAnimation(golemData.deathAnimation);
            Invoke(nameof(DestroyMe), golemData.deathAnimation.GetDuration());
        }
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}