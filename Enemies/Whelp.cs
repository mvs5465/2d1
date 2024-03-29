using UnityEngine;

public class Whelp : Entity
{
    public WhelpData whelpData;
    public FireballData fireballData;
    private SpriteAnimator spriteAnimator;
    private Rigidbody2D rb;
    private GameObject target;
    private bool coolingDown = false;
    private bool facingRight = false;
    private Vector3 startPos;

    private class WhelpAggroManager : MonoBehaviour
    {
        private Whelp parentWhelp;
        public static void Build(Whelp parentWhelp, float aggroRadius)
        {
            GameObject wamContainer = Utility.AttachChildObject(parentWhelp.gameObject, "WhelpAggroManager");
            wamContainer.AddComponent<WhelpAggroManager>().parentWhelp = parentWhelp;
            CircleCollider2D circleCollider2D = wamContainer.AddComponent<CircleCollider2D>();
            circleCollider2D.radius = aggroRadius;
            circleCollider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Wizard>())
            {
                parentWhelp.NotifyAggroEnter(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Wizard>())
            {
                parentWhelp.NotifyAggroExit();
            }
        }
    }

    protected override void StartCall()
    {
        spriteAnimator = SpriteAnimator.Build(gameObject, whelpData.idleAnimation);
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
        rb.drag = 0.9f;
        gameObject.AddComponent<CircleCollider2D>();
        WhelpAggroManager.Build(this, whelpData.aggroRadius);
        startPos = transform.position;
        gameObject.layer = whelpData.layer;
    }

    protected override void Update()
    {
        base.Update();
        if (target && !coolingDown)
        {
            coolingDown = true;
            Fireball.Throw(fireballData, transform.position, target.transform.position);
            Invoke(nameof(ClearCooldown), whelpData.fireballCooldown);
        }
        if (rb.velocity.x < 0 && facingRight)
        {
            facingRight = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (rb.velocity.x > 0 && !facingRight)
        {
            facingRight = true;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void ApproachTarget()
    {
        if (target && (target.transform.position - transform.position).magnitude > whelpData.aggroRadius / 2)
        {
            Vector3 towardsTarget = target.transform.position - transform.position;
            towardsTarget /= towardsTarget.magnitude;
            rb.AddForce(towardsTarget, ForceMode2D.Impulse);
        }
    }

    private void ReturnHome()
    {
        if ((startPos - transform.position).magnitude > whelpData.aggroRadius / 2)
        {
            Vector3 towardsHome = startPos - transform.position;
            towardsHome /= towardsHome.magnitude;
            rb.AddForce(towardsHome, ForceMode2D.Impulse);
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

    public void NotifyAggroEnter(GameObject target)
    {
        this.target = target;
        InvokeRepeating(nameof(ApproachTarget), 1, 1);
        CancelInvoke(nameof(ReturnHome));
    }

    public void NotifyAggroExit()
    {
        target = null;
        InvokeRepeating(nameof(ReturnHome), 1, 1);
        CancelInvoke(nameof(ApproachTarget));
    }
}