using UnityEngine;

public class Browner : Entity
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    protected override void StartCall()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
        // rb.drag = 

        gameObject.AddComponent<CircleCollider2D>();
        AggroManager.BuildAndAttach(this, 15);
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
        Wizard wizard = FindObjectOfType<Wizard>();
        wizard.GrantMoney(maxHealth);
        base.Die();
    }
}