using UnityEngine;

public class Sword : MonoBehaviour
{
    public GameConfig gameConfig;
    private SpriteAnimator spriteAnimator;
    private CircleCollider2D swordCollider;
    private bool attacking = false;
    private float attackDuration = 0.3f;

    public static Sword Build(GameObject parent, GameConfig gameConfig)
    {
        GameObject swordContainer = Utility.AttachChildObject(parent, "SwordContainer");
        // swordContainer.transform.localScale = Vector2.one * 1.5f;
        Sword sword = swordContainer.AddComponent<Sword>();
        sword.gameConfig = gameConfig;
        return sword;
    }

    private void Start()
    {
        spriteAnimator = SpriteAnimator.Build(gameObject, gameConfig.swordIdleAnimation, gameConfig.effectSortingLayer);
        swordCollider = gameObject.AddComponent<CircleCollider2D>();
        swordCollider.isTrigger = true;
        swordCollider.enabled = false;
        swordCollider.offset = new Vector2(0.75f, 0); ;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!attacking) Invoke(nameof(StartAttack), 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Browner browner = other.GetComponent<Browner>();
        if (browner)
        {
            browner.Damage(1);
            Vector2 knockback = browner.transform.position - transform.position;
            knockback /= knockback.magnitude;
            browner.GetComponent<Rigidbody2D>().AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    private void StartAttack()
    {
        attacking = true;
        spriteAnimator.SetAnimation(gameConfig.swordAttackAnimation);
        swordCollider.enabled = true;
        Invoke(nameof(StopAttack), attackDuration);
    }

    private void StopAttack()
    {
        attacking = false;
        swordCollider.enabled = false;
        spriteAnimator.SetAnimation(gameConfig.swordIdleAnimation);
    }
}