using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable.Characters;

namespace Breach.Placeable.Component
{
    public class HealthSystem : MonoBehaviour, IDamageable
    { //TODO consider character knowing what this is

        [SerializeField] float maxHealthPoints = 5f;
        [SerializeField] Image healthBar;

        const string TAKE_DAMAGE_TRIGGER = "TakeDamage";
        const string IS_ALIVE_BOOL = "IsAlive";

        float currentHealthPoints = 0f;
        Animator animator;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public float GetMaxHealth()
        {
            return maxHealthPoints;
        }
        public float GetCurrentHealth()
        {
            return currentHealthPoints;
        }

        public void Init()
        {
            if (currentHealthPoints <= -1)
            {
                currentHealthPoints = maxHealthPoints;
            }
            animator = GetComponent<Animator>();
            UpdateHealthBar();

            if(currentHealthPoints > 0)
            {
                animator.SetBool(IS_ALIVE_BOOL, true);
            }
            else
            {
                animator.SetBool(IS_ALIVE_BOOL, false);
            }
            TakeDamage(0);
        }

        public void SetMaxHealth(float maxHealth)
        {
            maxHealthPoints = maxHealth;
        }

        public void SetCurrentHealth(float currentHealth)
        {
            currentHealthPoints = currentHealth;
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
            animator.SetTrigger(TAKE_DAMAGE_TRIGGER);
            if (characterDies)
            {
                animator.SetBool(IS_ALIVE_BOOL, false);
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
}