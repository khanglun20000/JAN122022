using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HealthSystem
{
    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            if(currentHealth <= 0)
            {
                HealthOut?.Invoke();
            }
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

    [SerializeField] int currentHealth;


    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            CurrentHealth += value;
            HealthChanged?.Invoke();
        }
    }
    [SerializeField] int maxHealth;

    public delegate void OnHealthChanged();
    public OnHealthChanged HealthChanged;

    public delegate void OnOutOfHealth();
    public OnOutOfHealth HealthOut;

    public delegate void OnTakeDamage();
    public OnTakeDamage DamageTaken;

    public bool CanRegen = true;
    public float regenAmountPerSecond = 1;
    int floorRegenAmount;
    float regenHealth;

    int regenThreshold;
    private void SetRegenThreshold(int value)
    {
        regenThreshold = value;
        if (regenThreshold > maxHealth)
        {
            regenThreshold = maxHealth;
        }
    }

    public HealthSystem(int _maxHealth)
    {
        MaxHealth = _maxHealth;
        CurrentHealth = _maxHealth;
        SetRegenThreshold(_maxHealth);
    }

    public void TakeDamage(int _damage)
    {
        CurrentHealth -= _damage;
        SetRegenThreshold(regenThreshold - Mathf.RoundToInt(_damage/4));
        HealthChanged?.Invoke();
        DamageTaken?.Invoke();
    }

    public void Heal(int _amount)
    {
        if (currentHealth < maxHealth)
        {
            CurrentHealth += _amount;
            SetRegenThreshold(regenThreshold + _amount);
            if(currentHealth > maxHealth)
            {
                CurrentHealth = maxHealth;
            }
        }
    }

    public float GetHealthRatio()
    {
        return (float)currentHealth / maxHealth;
    }

    public float GetRegenThreshHoldRatio()
    {
        return (float)regenThreshold / maxHealth;
    }

    public void RegenerateHealth()
    {
        if (CanRegen && currentHealth < regenThreshold)
        {
            regenHealth += regenAmountPerSecond * Time.deltaTime;

            if(regenHealth >= 1)
            {
                floorRegenAmount = Mathf.FloorToInt(regenHealth);
                CurrentHealth += floorRegenAmount;
                regenHealth -= floorRegenAmount;
                HealthChanged?.Invoke();
            }
        }
        else if(currentHealth >= maxHealth)
        {
            CurrentHealth = maxHealth;
            CanRegen = false;
        }
    }

    public void SetRegenAmountPerSecond(float _amount)
    {
        regenAmountPerSecond += _amount;
    }
}
