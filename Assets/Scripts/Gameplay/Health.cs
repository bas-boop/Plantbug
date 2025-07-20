using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Range(1, 1000)] private int health;

        [SerializeField] private UnityEvent onTakeDamage = new();
        [SerializeField] private UnityEvent onDie = new();
        [SerializeField] private UnityEvent onHeal = new();
        [SerializeField] private UnityEvent onResurrect = new();

        public int CurrentHealth { get; private set; }

        private void Awake() => CurrentHealth = health;

        /// <summary>
        /// This object will take damage by the given amount. Will invoke the onTakeDamage event.
        /// When reaching zero health it will die.
        /// </summary>
        /// <param name="damage">Amount of damage to take.</param>
        public void TakeDamage(int damage)
        {
            if (CurrentHealth <= 0)
                return;

            CurrentHealth -= damage;
            onTakeDamage?.Invoke();
            
            if (CurrentHealth <= 0)
                Die();
        }

        /// <summary>
        /// Heals this object, does not over heal. Will invoke the onHeal event.
        /// </summary>
        /// <param name="amountToHeal"></param>
        public void Heal(int amountToHeal)
        {
            if (CurrentHealth <= 0)
                return;

            CurrentHealth += amountToHeal;
            onHeal?.Invoke();

            if (CurrentHealth > health)
                CurrentHealth = health;
        }

        /// <summary>
        /// Resurrects this object, health would be the max or the given value if given. Will invoke the onResurrect event.
        /// </summary>
        /// <param name="targetHealth"></param>
        public void Resurrect(int? targetHealth)
        {
            if (CurrentHealth > 0)
                return;
            
            CurrentHealth = targetHealth ?? health;
            onResurrect?.Invoke();
        }

        public int GetStartHealth() => health;

        private void Die()
        {
            CurrentHealth = 0;
            onDie?.Invoke();
        }
    }
}