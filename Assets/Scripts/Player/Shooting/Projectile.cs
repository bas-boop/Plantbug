using UnityEngine;

using Gameplay;

namespace Player.Shooting
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private float speed = 2;
        [SerializeField] private int damage = 1;
        [SerializeField] private float despawnTime = 15;

        private void Start()
        {
            rigidbody.linearVelocity = transform.up * speed;
            Invoke(nameof(Des), despawnTime);
        }

        public void DoDamage(GameObject hitTarget)
        {
            hitTarget.TryGetComponent(out Health h);

            if (h != null)
                h.TakeDamage(damage);
        }

        private void Des() => Destroy(gameObject);
    }
}