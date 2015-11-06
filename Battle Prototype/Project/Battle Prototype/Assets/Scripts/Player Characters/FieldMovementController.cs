using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;
using AnimationEvent = Scripts.Event_Dispatchers.AnimationEvent;

namespace Scripts.Player_Characters
{
    public class FieldMovementController
    {
        private MotionEngine _motionEngine;
        private DisplayController _displayController;
        private StatusEventDispatcher _statusEventDispatcher;

        private Transform _transform;
        private bool _isActive;
        private bool _isMoving;
        private bool _autoActionBlocking;
        private bool _hurtSequenceBlocking;
        private Vector3 _movementTarget;

        public Transform Transform { set { _transform = value; _movementTarget = value.position; } }

        public FieldMovementController(MotionEngine motionEngine, DisplayController displayController, StatusEventDispatcher statusEventDispatcher)
        {
            _motionEngine = motionEngine;
            _displayController = displayController;
            _statusEventDispatcher = statusEventDispatcher;

            _isActive = false;
            _isMoving = false;
            _autoActionBlocking = false;
            _hurtSequenceBlocking = false;
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
            FieldClickEventDispatcher.FieldClickHandler += HandleFieldClick;
            AnimationEventDispatcher.AnimationEventHandler += HandleAnimationEvent;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
            FieldClickEventDispatcher.FieldClickHandler -= HandleFieldClick;
            AnimationEventDispatcher.AnimationEventHandler -= HandleAnimationEvent;
        }

        public void Update()
        {
            if ((_autoActionBlocking) || (_hurtSequenceBlocking))
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
                case StatusMessage.ReduceHealth: UpdateHurtMoveBlockState(target, true); break;
            }
        }

        private void HandleFieldClick(Vector3 clickLocation)
        {
            if (_isActive)
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

        private void UpdateHurtMoveBlockState(Transform target, bool block)
        {
            if (_transform == target)
            {
                _hurtSequenceBlocking = block;
            }
        }

        private void HandleAnimationEvent(Transform originator, AnimationEvent message)
        {
            if (message == AnimationEvent.HurtSequenceComplete)
            {
                UpdateHurtMoveBlockState(originator, false);
            }
        }

        private const float Movement_Target_Stopping_Distance = 0.05f;
    }
}
