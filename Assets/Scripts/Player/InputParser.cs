using UnityEngine;
using UnityEngine.InputSystem;

using Player.Movement;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputParser : MonoBehaviour
    {
        [SerializeField] private Walk walk;
        [SerializeField] private Jump jump;
        
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
            walk.SetInput(input);
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
        }

        private void RemoveListeners()
        {
            _inputActionAsset["Jump"].performed -= JumpAction;
            _inputActionAsset["WallClimb"].performed -= WallClimbAction;
            _inputActionAsset["Shoot"].performed -= ShootAction;
        }
        
        #region Context

        private void JumpAction(InputAction.CallbackContext context) => walk.Jump(); // jump.DoJump();
        private void WallClimbAction(InputAction.CallbackContext context) => walk.SwitchWall();
        private void ShootAction(InputAction.CallbackContext context) => Debug.Log("Make shoot");

        #endregion
    }
}