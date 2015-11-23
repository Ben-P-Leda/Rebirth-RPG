using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class FieldMovementController
    {
        private MotionEngine _motionEngine;
        private DisplayController _displayController;
        private StatusEventDispatcher _statusEventDispatcher;

        private Transform _transform;
        private bool _isActive;
        private bool _movementFrozen;
        private bool _isMoving;
        private bool _autoActionBlocking;
        private float _movementBlockDuration;
        private Vector3 _movementTarget;

        public Transform Transform { set { _transform = value; _movementTarget = value.position; } }
        public bool IgnoreFieldClickEvents { set { _movementFrozen = value; EndFieldMovement(); } }

        public FieldMovementController(MotionEngine motionEngine, DisplayController displayController, StatusEventDispatcher statusEventDispatcher)
        {
            _motionEngine = motionEngine;
            _displayController = displayController;
            _statusEventDispatcher = statusEventDispatcher;

            _isActive = false;
            _movementFrozen = false;
            _isMoving = false;
            _autoActionBlocking = false;
            _movementBlockDuration = 0.0f;
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
            FieldClickEventDispatcher.FieldClickHandler += HandleFieldClick;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
            FieldClickEventDispatcher.FieldClickHandler -= HandleFieldClick;
        }

        public void Update()
        {
            if ((_autoActionBlocking) || (_movementBlockDuration > 0.0f))
            {
                _motionEngine.StopMoving();
            }
            else if (_isMoving)
            {
                Vector2 vectorToTarget = _movementTarget - _transform.position;

                if (vectorToTarget.magnitude > Movement_Target_Stopping_Distance)
                {
                    _motionEngine.MoveTowardsPosition(_movementTarget);
                    _displayController.IsMoving = true;
                    _displayController.SetFacing(_movementTarget);
                }
                else
                {
                    EndFieldMovement();
                    _statusEventDispatcher.FireStatusEvent(StatusMessage.CompletedFieldMovement);
                }
            }

            _movementBlockDuration = Mathf.Max(_movementBlockDuration - Time.deltaTime, 0.0f);
        }

        private void EndFieldMovement()
        {
            _motionEngine.StopMoving();
            _displayController.IsMoving = false;
            _isMoving = false;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.CharacterActivated: _isActive = (_transform == target); break;
                case StatusMessage.CharacterDeactivated: _isActive = false; break;
                case StatusMessage.EnemyActionTargetSelected: AbortMovementIfOwnTargetSpecified(originator, target); break;
                case StatusMessage.AlliedActionTargetSelected: AbortMovementIfOwnTargetSpecified(target, originator); break;
                case StatusMessage.StartedAutoAction: UpdateAutoActionMoveBlockState(target, true); break;
                case StatusMessage.CompletedAutoAction: UpdateAutoActionMoveBlockState(target, false); break;
                case StatusMessage.ReduceHealth: SetMoveBlockDuration(target, 0.5f); break;
            }
        }

        private void HandleFieldClick(Vector3 clickLocation)
        {
            if ((!_movementFrozen) && (_isActive))
            {
                _movementTarget = new Vector3(clickLocation.x, clickLocation.y, _transform.position.z);
                _statusEventDispatcher.FireStatusEvent(StatusMessage.StartedFieldMovement);
                _isMoving = true;
            }
        }

        private void AbortMovementIfOwnTargetSpecified(Transform actionExecuter, Transform actionTarget)
        {
            if (actionExecuter == _transform)
            {
                EndFieldMovement();
            }
        }

        private void UpdateAutoActionMoveBlockState(Transform target, bool block)
        {
            if (_transform == target)
            {
                _autoActionBlocking = block;
            }
        }

        private void SetMoveBlockDuration(Transform target, float duration)
        {
            if (_transform == target)
            {
                _movementBlockDuration = Mathf.Max(_movementBlockDuration, duration);
            }
        }

        private const float Movement_Target_Stopping_Distance = 0.05f;
    }
}
