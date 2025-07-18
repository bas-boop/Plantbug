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

        private int _currentHealth;

        private void Awake() => _currentHealth = health;

        /// <summary>
        /// This object will take damage by the given amount. Will invoke the onTakeDamage event.
        /// When reaching zero health it will die.
        /// </summary>
        /// <param name="damage">Amount of damage to take.</param>
        public void TakeDamage(int damage)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth -= damage;
            onTakeDamage?.Invoke();
            
            if (_currentHealth <= 0)
                Die();
        }

        /// <summary>
        /// Heals this object, does not over heal. Will invoke the onHeal event.
        /// </summary>
        /// <param name="amountToHeal"></param>
        public void Heal(int amountToHeal)
        {
            if (_currentHealth <= 0)
                return;

            _currentHealth += amountToHeal;
            onHeal?.Invoke();

            if (_currentHealth > health)
                _currentHealth = health;
        }

        /// <summary>
        /// Resurrects this object, health would be the max or the given value if given. Will invoke the onResurrect event.
        /// </summary>
        /// <param name="targetHealth"></param>
        public void Resurrect(int? targetHealth)
        {
            if (_currentHealth > 0)
                return;
            
            _currentHealth = targetHealth ?? health;
            onResurrect?.Invoke();
        }

        private void Die()
        {
            _currentHealth = 0;
            onDie?.Invoke();
        }
    }
}