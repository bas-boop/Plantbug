using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CustomGravity2D : MonoBehaviour
    {
        public Vector2 gravityDirection = Vector2.down;
        public float gravityStrength = 9.81f;
        public bool useCustomGravity = true;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
        }

        private void FixedUpdate()
        {
            if (!useCustomGravity)
                return;
            
            _rb.AddForce(gravityDirection.normalized * gravityStrength);
        }

        public void SetGravityDirection(Vector2 newDirection)
        {
            gravityDirection = newDirection.normalized;
        }

        public void FlipGravity()
        {
            gravityDirection = -gravityDirection;
        }
    }

}