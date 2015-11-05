using UnityEngine;
using Scripts.Event_Dispatchers;
using AnimationEvent = Scripts.Event_Dispatchers.AnimationEvent;
using Scripts.All_Characters;

namespace Scripts.All_Characters
{
    public class AutoActionController
    {
        private MotionEngine _motionEngine;
        private DisplayController _displayController;
        private StatusEventDispatcher _statusEventDispatcher;

        private bool _isActive;
        private bool _actionInProgress;
        private bool _fieldMovementInProgress;
        private Transform _actionTarget;
        private Vector3 _actionLocation;
        private float _cooldownRemaining;

        public Transform Transform { private get; set; }
        public Rigidbody2D Rigidbody2D { private get; set; }
        public float ActionLocationOffset { private get; set; }
        public Vector2 RequiredTargetProximity { private get; set; }
        public float Cooldown { private get; set; }
        public StatusMessage ActionInvokationStatusEvent { private get; set; }
        public float ActionEffectValue { private get; set; }

        public AutoActionController(MotionEngine motionEngine, DisplayController displayController, StatusEventDispatcher statusEventDispatcher)
        {
            _motionEngine = motionEngine;
            _displayController = displayController;
            _statusEventDispatcher = statusEventDispatcher;

            _isActive = false;
            _actionInProgress = false;
            _fieldMovementInProgress = false;
            _actionTarget = null;
            _actionLocation = Vector3.zero;
            _cooldownRemaining = 0.0f;

            ActionLocationOffset = 0;
            RequiredTargetProximity = new Vector2(Default_Required_Proximity, Default_Required_Proximity);
            Cooldown = Default_Cooldown;
            ActionInvokationStatusEvent = StatusMessage.ReduceHealth;
            ActionEffectValue = 1.0f;
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
            AnimationEventDispatcher.AnimationEventHandler += HandleAnimationEvent;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
            AnimationEventDispatcher.AnimationEventHandler -= HandleAnimationEvent;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.CharacterActivated: _isActive = (Transform == target); break;
                case StatusMessage.CharacterDeactivated: _isActive = false; break;
                case StatusMessage.StartedFieldMovement: HandleFieldMovementStart(); break;
                case StatusMessage.CompletedFieldMovement: HandleFieldMovementCompletion(target); break;
                case StatusMessage.EnemyActionTargetSelected: HandleActionTargetAssignment(target); break;
                case StatusMessage.AlliedActionTargetSelected: HandleActionTargetAssignment(originator); break;
                case StatusMessage.CharacterDead: HandleCharacterDeath(originator); break;
            }
        }

        private void HandleFieldMovementStart()
        {
            if (_isActive)
            {
                _fieldMovementInProgress = true;
            }
        }

        private void HandleFieldMovementCompletion(Transform characterCompletingMovement)
        {
            if (characterCompletingMovement == Transform)
            {
                _fieldMovementInProgress = false;
                if ((_actionTarget != null) && (!CloseEnoughToTarget()))
                {
                    _actionTarget = null;
                    _displayController.CompleteAutoAction();
                }
            }
        }

        private void HandleActionTargetAssignment(Transform target)
        {
            if (_isActive)
            {
                _actionTarget = target;
                _fieldMovementInProgress = false;
            }
        }

        private void HandleCharacterDeath(Transform deadCharacter)
        {
            if (deadCharacter == _actionTarget)
            {
                _actionTarget = null;
            }
        }

        public void Update()
        {
            if ((_actionTarget != null) && (!_fieldMovementInProgress))
            {
                if (!_actionInProgress)
                {
                    _actionLocation = CalculateActionLocation();
                    _displayController.SetFacing(_actionTarget.position);
                }

                if (CloseEnoughToTarget())
                {
                    _motionEngine.StopMoving();
                    _displayController.IsMoving = false;

                    if (_cooldownRemaining <= 0.0f)
                    {
                        StartAutoActionSequence();
                    }
                }
                else if (!_actionInProgress)
                {
                    _motionEngine.MoveTowardsPosition(_actionLocation);
                    _displayController.IsMoving = true;
                }
            }

            _cooldownRemaining = Mathf.Max(_cooldownRemaining - Time.deltaTime, 0.0f);
        }

        private Vector3 CalculateActionLocation()
        {
            float x = (Transform.position.x > _actionTarget.position.x)
                ? _actionTarget.position.x + ActionLocationOffset
                : _actionTarget.position.x - ActionLocationOffset;

            return new Vector3(x, _actionTarget.position.y, 0.0f);
        }

        private bool CloseEnoughToTarget()
        {
            return (WithinRange(Transform.position.x, _actionLocation.x, RequiredTargetProximity.x)
                && WithinRange(Transform.position.y, _actionLocation.y, RequiredTargetProximity.y));
        }

        private bool WithinRange(float position, float target, float range)
        {
            return ((position >= target - range) && (position <= target + range));
        }

        private void StartAutoActionSequence()
        {
            _actionInProgress = true;
            _statusEventDispatcher.FireStatusEvent(StatusMessage.StartedAutoAction);
            _displayController.TriggerAutoAction();
        }

        private void HandleAnimationEvent(Transform originator, AnimationEvent message)
        {
            if (originator == Transform)
            {
                switch (message)
                {
                    case AnimationEvent.AutoActionEffectOccurs: InvokeAutoActionEffect(); break;
                    case AnimationEvent.AutoActionComplete: CompleteAutoAction(); break;
                }
            }
        }

        private void InvokeAutoActionEffect()
        {
            _statusEventDispatcher.FireStatusEvent(_actionTarget, ActionInvokationStatusEvent, ActionEffectValue);
        }

        private void CompleteAutoAction()
        {
            _actionInProgress = false;
            _statusEventDispatcher.FireStatusEvent(StatusMessage.CompletedAutoAction);
            _displayController.CompleteAutoAction();
            _cooldownRemaining = Cooldown;
        }

        private float Default_Cooldown = 2.0f;
        private const float Default_Required_Proximity = 0.05f;
    }
}
