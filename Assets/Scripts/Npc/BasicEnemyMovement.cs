﻿using UnityEngine;

namespace Npc
{
    public class BasicEnemyMovement : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float speed = 2;
        [SerializeField, Tooltip("Move to the right")] private float moveRange = 3;
        
        [Header("Debug")]
        [SerializeField] private Color gizmosColor = Color.red;
        [SerializeField] private float dotSize = 0.1f;

        private Vector3 _startPos;
        private float _localTimer;
        private bool _isWalking = true;

        protected virtual void Start() => SetStartPosition();

        protected virtual void FixedUpdate()
        {
            if (!_isWalking)
                return;
            
            _localTimer += Time.fixedDeltaTime;
            Move(_localTimer);
        }
        
        protected void SetWalking(bool walking) => _isWalking = walking;

        private void SetStartPosition() => _startPos = transform.position;

        private void Move(float time)
        {
            float pingPong = Mathf.PingPong(time * speed, moveRange);
            Vector3 newPosition = _startPos + Vector3.right * pingPong;
            transform.position = newPosition;
        }

        private void OnDrawGizmos()
        {
            Vector3 from = Application.isPlaying ? _startPos : transform.position;
            Vector3 to = from + Vector3.right * moveRange;

            Gizmos.color = gizmosColor;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawSphere(from, dotSize);
            Gizmos.DrawSphere(to, dotSize);
        }
    }
}