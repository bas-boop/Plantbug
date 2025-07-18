using UnityEngine;

using Framework.Extensions;

namespace Npc
{
    public sealed class BasicEnemy : MonoBehaviour
    {
        [SerializeField] private float speed = 2;
        [SerializeField, Tooltip("Move to the right")] private float moveRange = 3;
        [SerializeField] private Color gizmosColor = Color.red;

        private Vector3 _startPos;
        private Vector3 _rangePos;
        
        private void FixedUpdate()
        {
            // move back and forth between _startPos & _rangePos
        }

        private void OnDrawGizmos()
        {
            Vector3 to = transform.position;
            to.AddX(moveRange);
            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(transform.position, to);
        }
    }
}