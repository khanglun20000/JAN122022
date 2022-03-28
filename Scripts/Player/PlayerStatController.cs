using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour
{

    [SerializeField] Image healthBarImg;
    [SerializeField] int maxHealth;
    [SerializeField] Image healthRegenThreshold;
    [SerializeField] HealthSystem healthSystem;

    [SerializeField] ExpSystem expSystem;

    [SerializeField] float startTimeAfterInjured;
    [SerializeField] float timeAfterInjured;

    public static PlayerStatController instance;
    public bool IsBattling
    {
        set
        {
            if(value == false)
            {
                MinimapController.instance.ShowMinimap();
                healthSystem.regenAmountPerSecond *= 5;
            }
            else
            {
                MinimapController.instance.HideMinimap();
                healthSystem.regenAmountPerSecond /= 5;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetUpHealth();
        SetUpExp();
    }

    // Update is called once per frame
    void Update()
    {
        RegenHealthAfterInjured();
    }

    void SetUpHealth()
    {
        healthSystem = new HealthSystem(maxHealth);
        healthBarImg.fillAmount = 1;
        healthSystem.HealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.DamageTaken += HealthSystem_OnTakeDamage;
    }

    void HealthSystem_OnHealthChanged()
    {
        healthBarImg.fillAmount = healthSystem.GetHealthRatio();
        healthRegenThreshold.fillAmount = healthSystem.GetRegenThreshHoldRatio();
    }

    void SetUpExp()
    {
        expSystem = new ExpSystem();
        UIManager.instance.UpdateExpCount(expSystem.CurrentExp);
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }

    public ExpSystem GetExpSystem()
    {
        return expSystem;
    }

    void RegenHealthAfterInjured()
    {
        if (timeAfterInjured < 0)
        {
            healthSystem.CanRegen = true;
            healthSystem.RegenerateHealth();
        }
        else
        {
            timeAfterInjured -= Time.deltaTime;
        }
    }

    void HealthSystem_OnTakeDamage()
    {
        timeAfterInjured = startTimeAfterInjured;
    }
}
