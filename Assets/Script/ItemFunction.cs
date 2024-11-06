using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunction : MonoBehaviour
{
    [Header("Health Item")]
    public float instantHealAmount; 
    public float minHealOverTime = 1f; 
    public float maxHealOverTime = 5f; 
    public float healDuration;

    [Header("Stamina Item")]
    public float staminaRestoreAmount = 100f;

    [Header("Particle System")]
    public GameObject healingFirst;
    public GameObject healingSecond;
    public GameObject energy;
    public GameObject resistance;

    [Header("Transform Particle System")]
    public Transform itemEffect;

    private HealthBar healthBar;
    private Stamina stamina;

    private void Start()
    {
        healthBar = FindAnyObjectByType<HealthBar>();
        stamina = GetComponent<Stamina>();


    }

    // ITEM HEAL
    public void UseHealthItem()
    {    
            GameObject heal = Instantiate(healingFirst, itemEffect.position, Quaternion.Euler(-90f, 0f, 0f));
            StartCoroutine(FollowPlayer(heal));
            Destroy(heal, 2f);
            healthBar.Heal(instantHealAmount);

            StartCoroutine(HealOverTime());     
    }

    private IEnumerator HealOverTime()
    {
        StartCoroutine(HealingPerSecond());
        float elapsedTime = 0f;
        while (elapsedTime < healDuration)
        {
            float healAmount = Random.Range(minHealOverTime, maxHealOverTime);

            healthBar.Heal(healAmount);

            yield return new WaitForSeconds(1f);

            elapsedTime += 1f;
        }
    }

    // ITEM STAMINA
    public void UseStaminaItem(Stamina staminaBar)
    {
        GameObject energyPre = Instantiate(energy, itemEffect.position, Quaternion.Euler(-90f, 0f, 0f));
        StartCoroutine(FollowPlayer(energyPre));
        Destroy(energyPre,2f);

        if (staminaBar != null)
        {
            staminaBar.RestoreStamina(staminaRestoreAmount);
        }
    }

    // ITEM INMUNE
    public void UseImmunityItem()
    {
        GameObject resistanceEff = Instantiate(resistance, itemEffect.position, Quaternion.Euler(-90f, 0f, 0f));
        StartCoroutine(FollowPlayer(resistanceEff));
        Destroy(resistanceEff, 7f);

        StatusEffects statusEffects = FindAnyObjectByType<StatusEffects>();
        if (statusEffects != null)
        {
            statusEffects.ApplyImmunity(7f);
        }
    }

    IEnumerator HealingPerSecond()
    {
        GameObject healPer = Instantiate(healingSecond, itemEffect.position, Quaternion.Euler(-90f, 0f, 0f));
        StartCoroutine(FollowPlayer(healPer));
        yield return new WaitForSeconds(5f);
        Destroy(healPer);
    }

    private IEnumerator FollowPlayer(GameObject particle)
    {
        while (particle != null)
        {
            particle.transform.position = itemEffect.position; 
            yield return null; 
        }
    }

}
