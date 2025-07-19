using System;
using UnityEngine;
using UnityEngine.Events;

using Framework.Attributes;
using Framework.Extensions;

namespace Framework.TriggerArea
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public sealed class TriggerArea : MonoBehaviour
    {
        [SerializeField] private StandardSprites shapeToUse;
        [SerializeField, Tag] private string tagToTriggerWith = "Player";
        [SerializeField] private TriggerBehaviour behaviour;
        [SerializeField] private bool isOneTimeUse;

        [Space(20)]
        [SerializeField] private UnityEvent<GameObject> onEnter = new();
        [SerializeField] private UnityEvent<GameObject> onExit = new();

        private BoxCollider2D _boxCollider;
        private CircleCollider2D _circleCollider;
        private CapsuleCollider2D _capsuleCollider;

        private bool _isTriggered;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _circleCollider = GetComponent<CircleCollider2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            _boxCollider.isTrigger = true;
            _circleCollider.isTrigger = true;
            _capsuleCollider.isTrigger = true;

            UpdateVisuals();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (behaviour == TriggerBehaviour.EXIT_ONLY
                || CheckOneTimeUse()
                || !other.CompareTag(tagToTriggerWith))
                return;

            _isTriggered = true;
            onEnter?.Invoke(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (behaviour == TriggerBehaviour.ENTER_ONLY
                || CheckOneTimeUse()
                || !other.CompareTag(tagToTriggerWith))
                return;

            _isTriggered = true;
            onExit?.Invoke(other.gameObject);
        }

        private void OnValidate() => UpdateVisuals();

        private void UpdateVisuals()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _circleCollider = GetComponent<CircleCollider2D>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            switch (shapeToUse)
            {
                case StandardSprites.SQUARE:
                    _boxCollider.enabled = true;
                    _circleCollider.enabled = false;
                    _capsuleCollider.enabled = false;
                    break;

                case StandardSprites.CIRCLE:
                    _boxCollider.enabled = false;
                    _circleCollider.enabled = true;
                    _capsuleCollider.enabled = false;
                    break;

                case StandardSprites.CAPSULE:
                    _boxCollider.enabled = false;
                    _circleCollider.enabled = false;
                    _capsuleCollider.enabled = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TestTrigger() => Debug.Log(shapeToUse);

        private bool CheckOneTimeUse() => isOneTimeUse && _isTriggered;
    }
}
