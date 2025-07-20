using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Npc
{
    public sealed class BossMeleeAttack : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform hitbox;
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] private float attackSpeed = 5f;
        [SerializeField] private float retreatSpeed = 3f;
        [SerializeField] private List<GameObject> cables;

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
            
            UpdateCableRotation(direction);
            UpdateCableColor(direction, false);

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
            UpdateCableColor(direction, true);
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

        private void UpdateCableRotation(Vector3 dir)
        {
            foreach (GameObject cable in cables)
            {
                cable.transform.rotation = Quaternion.Euler(0,0, dir == Vector3.right ? -90 : 90);
            }
        }

        private void UpdateCableColor(Vector3 dir, bool isGoingBack)
        {
            if (isGoingBack)
            {
                if (dir == Vector3.right)
                {
                    cables[0].SetActive(false);
                    cables[1].SetActive(true);
                    cables[2].SetActive(false);
                    cables[3].SetActive(false);
                }
                else
                {
                    cables[0].SetActive(false);
                    cables[1].SetActive(false);
                    cables[2].SetActive(false);
                    cables[3].SetActive(true);
                }
                
                return;
            }
            
            if (dir == Vector3.right)
            {
                cables[0].SetActive(true);
                cables[1].SetActive(false);
                cables[2].SetActive(false);
                cables[3].SetActive(false);
            }
            else
            {
                cables[0].SetActive(false);
                cables[1].SetActive(false);
                cables[2].SetActive(true);
                cables[3].SetActive(false);
            }
        }
    }
}