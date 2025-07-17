using UnityEngine;

using Gameplay;

namespace Player.Movement
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rioRigidbody2D;
        [SerializeField] private UniversalGroundChecker groundChecker;
        [SerializeField] private float jumpStrength = 1;
        
        public void DoJump()
        {
            if (!groundChecker.IsGrounded)
                return;
            
            rioRigidbody2D.linearVelocityY = jumpStrength;
        }
    }
}