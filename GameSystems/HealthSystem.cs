using UnityEngine;
using System;

namespace Assets.Assets.Scripts.GameSystems
{
    [Serializable]
    public class HealthSystem
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int health;
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;
        public event Action OnHealed;
        public event Action OnDamaged;  
        public void Init()
        {
            health = maxHealth;
        }
        public void Heal(int amount)
        {
            if (amount <= 0) return;

            health += amount;
            health = Mathf.Min(health, maxHealth);
            OnHealthChanged?.Invoke(health, maxHealth);
            OnHealed?.Invoke();
        }

        public void Damage(int amount)
        {
            if (amount <= 0) return;

            health -= amount;
            health = Mathf.Max(health, 0);
            OnHealthChanged?.Invoke(health, maxHealth);
            OnDamaged?.Invoke();
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDied?.Invoke();
            Debug.Log("Unit died");
        }

        public int GetHealth() => health;
        public int GetMaxHealth() => maxHealth;
        public float GetHealthNormalized() => (float)health / maxHealth;
    }
}
