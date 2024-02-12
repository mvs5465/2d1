using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected int curHealth;
    public int maxHealth;
    protected int curEnergy;
    public int maxEnergy;
    public GameConfig gameConfig;

    private HealthbarBuilder.Healthbar healthbar;

    private void Start()
    {
        curHealth = maxHealth;
        if (HealthbarEnabled()) healthbar = HealthbarBuilder.AttachHealthbar(gameConfig, gameObject);
        StartCall();
    }
    protected virtual void Update()
    {
        if (transform.position.magnitude > 200)
        {
            Debug.Log(string.Format("{0} was killed by killbox!", name));
            Damage(100000000);
        }
    }
    public int GetHealth()
    {
        return curHealth;
    }
    public virtual void Damage(int amount)
    {
        curHealth = Math.Max(curHealth - amount, 0);
        if (healthbar) healthbar.SetRatio((float)curHealth / maxHealth);
        if (curHealth == 0) Die();
    }

    public int GetEnergy()
    {
        return curEnergy;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual bool HealthbarEnabled()
    {
        return true;
    }
    protected abstract void StartCall();
}
