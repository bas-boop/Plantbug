using UnityEngine;

using Framework.Extensions;

namespace Npc
{
    public sealed class BasicEnemyMovement : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float speed = 2;
        [SerializeField, Tooltip("Move to the right")] private float moveRange = 3;
        
        [Header("Debug")]
        [SerializeField] private Color gizmosColor = Color.red;
        [SerializeField] private float dotSize = 0.1f;

        private Vector3 _startPos;
        
        private void Start() => _startPos = transform.position;

        private void FixedUpdate()
        {
            float pingPong = Mathf.PingPong(Time.time * speed, moveRange);
            Vector3 newPosition = _startPos + Vector3.right * pingPong;
            transform.position = newPosition;
        }

        private void OnDrawGizmos()
        {
            Vector3 to = transform.position;
            to.AddX(moveRange);
            
            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(transform.position, to);
            Gizmos.DrawSphere(transform.position, dotSize);
            Gizmos.DrawSphere(to, dotSize);
        }
    }
}