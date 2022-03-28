using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ExpSystem
{
    public ExpSystem()
    {
        currentExp = 0;
        ExpChanged += UpdateExpUI;
    }

    [SerializeField] int currentExp;
    public int CurrentExp
    {
        get { return currentExp; }
        set
        {
            currentExp = value;
            ExpChanged();
        }
    }
    public delegate void OnExpChanged();
    public OnExpChanged ExpChanged;

    public void GainExp(int _amount)
    {
        currentExp += _amount;
        ExpChanged?.Invoke();
    }

    public void SpendExp(int _amount)
    {
        currentExp -= _amount;
        ExpChanged?.Invoke();
    }

    void UpdateExpUI()
    {
        UIManager.instance.UpdateExpCount(currentExp);
    }
}
