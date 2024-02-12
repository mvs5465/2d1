using UnityEngine;

public class Ninja : Pigbro
{
    public SpriteAnimatorData idleAnimation;
    public SpriteAnimatorData runAnimation;
    public SpriteAnimatorData attackAnimation;
    private SpriteAnimator spriteAnimator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private enum Weapon
    {
        STAR,
        SWORD
    }
    private Weapon currentWeapon;

    protected override void StartCall()
    {
        base.StartCall();
        spriteAnimator = SpriteAnimator.Build(gameObject, idleAnimation, gameConfig.playerSortingLayer);
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        transform.localScale *= 1.5f;

        Sword.Build(gameObject, gameConfig);
        Flashlight.Build(gameObject);
    }

    protected override void Update()
    {
        base.Update();
        SpriteAnimatorData animationToPlay = idleAnimation;
        if (Input.GetKey(KeyCode.A))
        {
            animationToPlay = runAnimation;
            if (facingRight)
            {
                Vector2 newScale = transform.localScale;
                newScale.x = -transform.localScale.x;
                transform.localScale = newScale;
                facingRight = false;
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animationToPlay = runAnimation;
            if (!facingRight)
            {
                Vector2 newScale = transform.localScale;
                newScale.x = -transform.localScale.x;
                transform.localScale = newScale;
                facingRight = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) FireWeapon();
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchWeapon();
        if (animationToPlay != spriteAnimator.GetAnimation()) spriteAnimator.SetAnimation(animationToPlay);
    }

    private void SwitchWeapon()
    {
        currentWeapon = currentWeapon == Weapon.SWORD ? currentWeapon = Weapon.STAR : currentWeapon = Weapon.SWORD;
    }
    private void FireWeapon()
    {
        if (currentWeapon == Weapon.SWORD)
        {
            // do nothing
        }
        else
        {
            Vector3 target = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
            ThrowingStar.Throw(gameObject, gameConfig, target);
        }
    }
}