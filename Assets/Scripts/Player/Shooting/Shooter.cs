using UnityEngine;

namespace Player.Shooting
{
    public sealed class Shooter : MonoBehaviour
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private float maxAngleFromUp = 45f;

        [SerializeField] private bool showGizmos;

        public void Shoot(Vector2 mousePosition)
        {
            Vector2 direction = CalculateDirection(mousePosition);

            Debug.Log($"Clamped Direction: {direction}");

            
        }

        private Vector2 CalculateDirection(Vector2 mousePosition)
        {
            Vector2 direction = (mousePosition - (Vector2) transform.position).normalized;

            float angle = Vector2.Angle(transform.up, direction);
            float sign = Mathf.Sign(Vector2.SignedAngle(transform.up, direction));

            if (angle <= maxAngleFromUp)
                return direction;
            
            float clampedAngle = maxAngleFromUp * sign;
            float radians = clampedAngle * Mathf.Deg2Rad;
            direction = new (Mathf.Sin(radians), Mathf.Cos(radians));

            return direction;
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;
            
            const float length = 10;
            Gizmos.color = Color.yellow;

            Vector3 origin = transform.position;
            Vector3 forward = transform.up.normalized;
            Vector3 leftDir = Quaternion.Euler(0, 0, -maxAngleFromUp) * forward;
            Vector3 rightDir = Quaternion.Euler(0, 0, maxAngleFromUp) * forward;

            Gizmos.DrawLine(origin, origin + leftDir * length);
            Gizmos.DrawLine(origin, origin + rightDir * length);
        }
    }
}