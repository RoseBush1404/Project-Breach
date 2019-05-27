using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {

    [SerializeField] float maxHealthPoints = 5f;
    [SerializeField] Image healthBar;

    float currentHealthPoints = 0f;
    Animator animator;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    void Start()
    {
        currentHealthPoints = maxHealthPoints;
        animator = GetComponent<Animator>();
    }

    private void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.fillAmount = healthAsPercentage;
        }
    }

    public void TakeDamage(float damage)
    {
        bool characterDies = (currentHealthPoints - damage <= 0);
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        UpdateHealthBar();
        if (characterDies)
        {
            GetComponent<Character>().HasDied();
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    public void Heal(float points)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        UpdateHealthBar();
    }
}
