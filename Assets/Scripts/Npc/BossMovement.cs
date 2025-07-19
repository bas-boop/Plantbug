using UnityEngine;

namespace Npc
{
    public class BossMovement : BasicEnemyMovement
    {
        [Header("Boss stats")]
        [SerializeField] private BossMeleeAttack attack;
        [SerializeField] private Vector2 randomIdleTime = new(1, 4);
        [SerializeField] private Vector2 randomTimeWalkFor = new(3, 5);

        private float _walkTimer;
        private float _currentWalkDuration;
        private bool _isWalking;

        protected override void Start()
        {
            base.Start();
            StartWalking();
        }

        protected override void FixedUpdate()
        {
            if (!_isWalking)
                return;
            
            _walkTimer += Time.fixedDeltaTime;
            base.FixedUpdate();

            if (_walkTimer < _currentWalkDuration)
                return;
            
            _isWalking = false;
            SetWalking(false);
            _walkTimer = 0;
            attack.Attack();
            Invoke(nameof(StartWalking), Random.Range(randomIdleTime.x, randomIdleTime.y));
        }

        private void StartWalking()
        {
            _currentWalkDuration = Random.Range(randomTimeWalkFor.x, randomTimeWalkFor.y);
            _isWalking = true;
            SetWalking(true);
        }
    }
}