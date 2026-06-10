using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isInvincible;
    public bool invincibilityOnTimer;

    public EventHandler OnDamageTaken;
    public EventHandler OnDeath;
    public EventHandler OnHealed;

    public bool cooldown;
    private float timer;
    [SerializeField] private float timerGoal;
    private bool isAlive;

    private void Awake()
    {
        isAlive = true;
    }

    private void Update()
    {
        if (cooldown)
        {
            timer += Time.deltaTime;
            if (timer > timerGoal)
            {
                cooldown = false;
            }
        }
    }

    public void SetHealth(int _val)
    {
        maxHealth = _val;
        currentHealth = _val;
    }

    public void SetCurrentHealth(int val)
    {
        currentHealth = val;
        OnHealed?.Invoke(this, System.EventArgs.Empty);
    }

    public void TakeDamage(int dmg)
    {
        if (!isInvincible && !cooldown && isAlive)
        {
            currentHealth -= dmg;
            OnDamageTaken?.Invoke(this, System.EventArgs.Empty);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }

    public void RestoreHealth(int _newHealth)
    {
        currentHealth += _newHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealed?.Invoke(this, EventArgs.Empty);
        cooldown = true;
        timer = 0f;
    }

    public void SetIsAlive(bool val)
    {
        isAlive = val;
    }
}
