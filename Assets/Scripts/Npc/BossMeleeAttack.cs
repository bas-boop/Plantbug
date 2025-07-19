using UnityEngine;
using System.Collections;

namespace Npc
{
    public sealed class BossMeleeAttack : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform hitbox;
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] private float attackSpeed = 5f;
        [SerializeField] private float retreatSpeed = 3f;

        private bool _isAttacking;

        public void Attack()
        {
            if (!_isAttacking)
                StartCoroutine(PerformAttack());
        }

        private IEnumerator PerformAttack()
        {
            _isAttacking = true;

            Vector3 startPos = hitbox.position;
            Vector3 direction = (player.position.x < hitbox.position.x) ? Vector3.left : Vector3.right;
            Vector3 targetPos = startPos + direction * attackDistance;

            // Attack phase
            hitbox.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * attackSpeed;
                hitbox.position = Vector3.Lerp(startPos, targetPos, elapsed);
                yield return null;
            }

            // Retreat phase
            elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * retreatSpeed;
                hitbox.position = Vector3.Lerp(targetPos, startPos, elapsed);
                yield return null;
            }

            hitbox.gameObject.SetActive(false);
            hitbox.position = startPos;
            _isAttacking = false;
        }
    }
}