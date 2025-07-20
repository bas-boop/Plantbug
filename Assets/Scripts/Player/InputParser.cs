using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

using Player.Movement;
using Player.Shooting;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(WallClimbPlatformer))]
    public sealed class InputParser : MonoBehaviour
    {
        [SerializeField] private WallClimbPlatformer wallClimbPlatformer;
        [SerializeField] private Shooter shooter;
        [SerializeField] private PlayerUnstucker playerUnstucker;
        
        private PlayerInput _playerInput;
        private InputActionAsset _inputActionAsset;
        
        private void Awake()
        {
            GetReferences();
            Init();
        }

        private void Update()
        {
            Vector2 input = _inputActionAsset["Move"].ReadValue<Vector2>();
            wallClimbPlatformer.SetInput(input);
        }

        private void OnEnable() => AddListeners();

        private void OnDisable() => RemoveListeners();

        private void GetReferences()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Init() => _inputActionAsset = _playerInput.actions;

        private void AddListeners()
        {
            _inputActionAsset["Jump"].performed += JumpAction;
            _inputActionAsset["WallClimb"].performed += WallClimbAction;
            _inputActionAsset["Shoot"].performed += ShootAction;
            _inputActionAsset["Unstuck"].performed += Unstuck;
        }

        private void RemoveListeners()
        {
            _inputActionAsset["Jump"].performed -= JumpAction;
            _inputActionAsset["WallClimb"].performed -= WallClimbAction;
            _inputActionAsset["Shoot"].performed -= ShootAction;
            _inputActionAsset["Unstuck"].performed -= Unstuck;
        }
        
        #region Context

        private void JumpAction(InputAction.CallbackContext context) => wallClimbPlatformer.Jump();
        private void WallClimbAction(InputAction.CallbackContext context) => wallClimbPlatformer.WallAction();
        private void ShootAction(InputAction.CallbackContext context)
        {
            Vector2 mousePos = _inputActionAsset["MousePosition"].ReadValue<Vector2>();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
            shooter.Shoot(mouseWorldPos);
        }
        private void Unstuck(InputAction.CallbackContext context)
        {
            if (playerUnstucker != null)
                playerUnstucker.Unstuck();
        }

        #endregion
    }
}