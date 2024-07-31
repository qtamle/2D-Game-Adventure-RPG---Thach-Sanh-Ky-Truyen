using UnityEngine;
using UnityEngine.UI;

public class HealthbarEnemy : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public float health;
    public float maxHealth = 100f;

    private void Start()
    {
        health = maxHealth;
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (slider != null)
        {
            slider.value = health;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
