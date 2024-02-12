using System;
using UnityEngine;

public class Pigbro : Entity
{
    public static string PIGBRO_TAG = "Player";
    private Rigidbody2D rb;
    private int money = 0;
    public int maxSpeed = 2;
    public void Reset()
    {
        rb.velocity = Vector2.zero;
        Time.timeScale = 1;
        curHealth = curHealth == 0 ? 1 : curHealth;
    }
    protected override void StartCall()
    {
        gameObject.tag = PIGBRO_TAG;
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1;
            rb.freezeRotation = true;
        }
        // PigLaserController.BuildAndAttach(gameObject, gameConfig);
        Debug.Log("Pigbro Ready");
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKey(KeyCode.D))
        {
            if (rb.velocity.x < maxSpeed) rb.AddForce(Vector2.right * 1f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (rb.velocity.x > -maxSpeed) rb.AddForce(Vector2.left * 1f);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.Space) && Math.Abs(rb.velocity.y) < 0.1) rb.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
    }

    protected override void Die()
    {
        Time.timeScale = 0;
        FindObjectOfType<UIController>().ReDraw();
        Debug.Log("Pigbro died!");
    }

    public override void Damage(int amount)
    {
        if (curHealth == 0) return;
        Debug.Log(string.Format("Pigbro Received Damage {0}", amount));
        base.Damage(amount);
        FindObjectOfType<UIController>().ReDraw();
    }

    public void GrantMoney(int amount)
    {
        money += amount;
        money = Math.Max(0, money + amount);
        FindObjectOfType<UIController>().ReDraw();
    }

    public int GetMoney()
    {
        return money;
    }

    protected override bool HealthbarEnabled()
    {
        return false;
    }
}
