using System;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public PlayerConfig playerConfig;
    public SpellInventory spellInventory;
    private SlopeDetector slopeDetectorRight;
    private SlopeDetector slopeDetectorLeft;
    public CandleData candleData;
    private SpriteAnimator spriteAnimator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private int curMaxSpeed;
    private int curHealth;
    private int maxHealth;
    private int money;
    private bool facingRight = true;
    private bool attacking = false;

    private void Start()
    {
        gameObject.layer = playerConfig.layer;
        transform.localScale = Vector2.one * playerConfig.sizeMultiplier;

        spriteAnimator = SpriteAnimator.Build(gameObject, playerConfig.idleAnimation, "Effect");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        curMaxSpeed = playerConfig.maxSpeed;
        curHealth = playerConfig.startHealth;
        maxHealth = playerConfig.maxHealth;
        UIManager.FindAndUpdateHealth(curHealth, maxHealth);

        gameObject.AddComponent<CapsuleCollider2D>();

        spellInventory = SpellInventory.Build(gameObject);
        slopeDetectorLeft = SlopeDetector.Build(gameObject, new Vector2(-0.382f, -0.33f), 0.1f);
        slopeDetectorLeft.gameObject.name = "SlopeDetectorLeft";
        slopeDetectorRight = SlopeDetector.Build(gameObject, new Vector2(0.085f, -0.33f), 0.1f);
        slopeDetectorRight.gameObject.name = "SlopeDetectorRight";
    }

    private void Update()
    {
        SpriteAnimatorData animationToPlay = playerConfig.idleAnimation;
        if (Input.GetKey(KeyCode.A))
        {
            if (facingRight) Flip();
            animationToPlay = playerConfig.runAnimation;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (!facingRight) Flip();
            animationToPlay = playerConfig.runAnimation;
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Math.Abs(rb.velocity.y) < 0.1f)
            {
                rb.AddForce(Vector2.up * playerConfig.jumpHeight);
            }
            animationToPlay = playerConfig.idleAnimation;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animationToPlay = playerConfig.attackAnimation;
            attacking = true;
            Vector3 targetPos = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
            // Frostbolt.Throw(frostboltData, transform.position + Vector3.down * spriteRenderer.size.y / 2, target);
            spellInventory.Cast(transform.position + Vector3.down * spriteRenderer.size.y / 2, targetPos);

            Invoke(nameof(EndAttack), 0.2f);
        }
        if (attacking) animationToPlay = playerConfig.attackAnimation;
        if (animationToPlay != spriteAnimator.GetAnimation()) spriteAnimator.SetAnimation(animationToPlay);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // Vector3 moveDirection = curMaxSpeed * Time.fixedDeltaTime * Vector2.left;
            // moveDirection += rb.gravityScale * Time.fixedDeltaTime * (Vector3)Physics2D.gravity;
            // rb.MovePosition(transform.position + moveDirection);
            Vector2 moveDirection = Vector2.left + Vector2.up * (slopeDetectorLeft.Detect() ? 1 : 0);
            if (rb.velocity.magnitude < curMaxSpeed) rb.AddForce(moveDirection, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Vector3 moveDirection = curMaxSpeed * Time.fixedDeltaTime * Vector2.right;
            // moveDirection += rb.gravityScale * Time.fixedDeltaTime * (Vector3)Physics2D.gravity;
            Vector2 moveDirection = Vector2.right + Vector2.up * (slopeDetectorRight.Detect() ? 1 : 0);
            if (rb.velocity.magnitude < curMaxSpeed) rb.AddForce(moveDirection, ForceMode2D.Impulse);
        }
    }

    private void EndAttack()
    {
        attacking = false;
    }
    private void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        facingRight = !facingRight;
    }

    public int GetHealth()
    {
        return curHealth;
    }
    public void Damage(int amount)
    {
        if (curHealth - amount > maxHealth)
        {
            curHealth = maxHealth;
        }
        else if (curHealth - amount < 0)
        {
            curHealth = 0;
            Die();
        }
        else
        {
            curHealth -= amount;
        }
        UIManager.FindAndUpdateHealth(curHealth, maxHealth);
    }
    public void GrantMoney(int amount)
    {
        money += amount;
        money = Math.Max(0, money + amount);
        UIManager.FindAndUpdateMoney(money);
    }

    private void Die()
    {
        Time.timeScale = 0;
        UIManager.FindAndUpdateHealth(curHealth, maxHealth);
    }

    public void Reset()
    {
        rb.velocity = Vector2.zero;
        Time.timeScale = 1;
        Damage(-(maxHealth - curHealth));
        UIManager.FindAndUpdateHealth(curHealth, maxHealth);
    }
}