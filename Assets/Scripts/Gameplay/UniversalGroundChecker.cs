using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    /// <summary>
    /// Universal ground checker supporting 2D and 3D environments with line and sphere casts.
    /// Supports both offset and transform-based ground checking.
    /// </summary>
    public sealed class UniversalGroundChecker : MonoBehaviour
    {
        #region SerializeField fields
        
        [Header("Usage")]
        [SerializeField, Tooltip("True for 3D settings.\nFalse for 2D settings.")] private bool is3D = true;
        [SerializeField, Tooltip("True for line.\nFalse for sphere.")] private bool lineOrSphere = true;
        [SerializeField, Tooltip("True for offset.\nFalse for transform.")] private bool offSetOrTransform;
        
        [Header("Settings")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float rayCastLength = 1f;
        [SerializeField] private float sphereCastRadius = 1f;
        [SerializeField] private float coyoteTime;
        [SerializeField] private Vector2 offSet2D;
        [SerializeField] private Vector3 offSet3D;
        [SerializeField] private Transform groundCheckerTransform;
        [SerializeField] private Vector2 direction2d = Vector2.down;
        [SerializeField] private Vector3 direction3d = Vector3.down;
        
        [Header("Debug")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool gizmos;
        [SerializeField] private Color gizmosColor = Color.cyan;
        
        #endregion

        #region Private fields

        private enum GroundedState
        {
            GROUNDED,
            AIRED
        }

        private bool _isOnGround = false;
        private bool _isLeavingGround = true;
        
        private GroundedState _currentState;

        private Vector2 _origin2D;
        private Vector3 _origin3D;

        #endregion

        #region Properties

        public bool IsGrounded { get => isGrounded; private set => isGrounded = value; }
        public bool IsCoyoteGrounded { get => isGrounded; private set => isGrounded = value; }

        #endregion
        
        #region Events
        
        [Space(20)]
        [SerializeField] private UnityEvent onGroundEnter = new ();
        [SerializeField] private UnityEvent onGroundLeave = new ();
        
        #endregion

        private void FixedUpdate()
        {
            CalculateGroundRayCasting();
            HandleStateTransitions();
        }

        public void SetDirection(Vector3 targetDir)
        {
            direction3d = targetDir;
            direction2d = targetDir;
        }

        public void SetOffset(Vector3 targetOffset)
        {
            offSet3D = targetOffset;
            offSet2D = targetOffset;
        }

        private void CalculateGroundRayCasting()
        {
            if (is3D)
                _origin3D = !offSetOrTransform
                    ? transform.position + offSet3D
                    : groundCheckerTransform != null ? groundCheckerTransform.position : Vector3.zero;
            else
                _origin2D = !offSetOrTransform
                    ? (Vector2)transform.position + offSet2D
                    : groundCheckerTransform != null ? groundCheckerTransform.position : Vector2.zero;
            

            IsGrounded = GetGround();

            if (IsGrounded)
                IsCoyoteGrounded = true;
            else
                Invoke(nameof(TurnCoyoteFalse), coyoteTime);
        }

        private bool GetGround()
        {
            return lineOrSphere 
                ? is3D
                    ? Physics.RaycastAll(_origin3D, direction3d, rayCastLength, groundLayer)
                        ?.Any(collider => collider.collider.gameObject != gameObject) ?? false
                    : Physics2D.RaycastAll(_origin2D, direction2d, rayCastLength, groundLayer)
                        ?.Any(collider => collider.collider.gameObject != gameObject) ?? false
                : is3D
                    ? Physics.OverlapSphere(_origin3D, sphereCastRadius, groundLayer)
                        ?.Any(collider => collider.gameObject != gameObject) ?? false
                    : Physics2D.OverlapCircleAll(_origin2D, sphereCastRadius, groundLayer)
                        ?.Any(collider => collider.gameObject != gameObject) ?? false;
        }

        private void TurnCoyoteFalse() => IsCoyoteGrounded = false;
        
        private void HandleStateTransitions()
        {
            _currentState = IsGrounded ? GroundedState.GROUNDED : GroundedState.AIRED;
            
            switch (_currentState)
            {
                case GroundedState.GROUNDED when !_isOnGround:
                    onGroundEnter?.Invoke();
                    _isOnGround = true;
                    _isLeavingGround = false;
                    break;
                case GroundedState.AIRED when !_isLeavingGround:
                    onGroundLeave?.Invoke();
                    _isLeavingGround = true;
                    _isOnGround = false;
                    break;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!gizmos)
                return;

            Vector3 origin = is3D ? _origin3D : _origin2D;
            Vector3 dirctoin = is3D ? direction3d : direction2d;
            Gizmos.color = gizmosColor;
            
            if (lineOrSphere)
            {
                Vector3 endPosition = origin + dirctoin * rayCastLength;
                Gizmos.DrawLine(origin, endPosition);
            }
            else
                Gizmos.DrawWireSphere(origin, sphereCastRadius);
        }
    }
}