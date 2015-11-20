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

        private Transform _actionTarget;
        public bool _actionInProgress;
        private bool _actionEffectHasFired;
        private Vector3 _actionLocation;
        private float _cooldownRemaining;
        private bool _autoActionDisabled;
        private float _actionBlockDuration;

        public Transform Transform { private get; set; }
        public Rigidbody2D Rigidbody2D { private get; set; }
        public bool HasTarget { get { return _actionTarget != null; } }
        public bool AutoActionDisabled { set { SetAutoActionDisabledState(value); } }

        public float ActionLocationOffset { private get; set; }
        public Vector2 RequiredTargetProximity { private get; set; }
        public float Cooldown { private get; set; }
        public StatusMessage ActionInvokationStatusEvent { private get; set; }
        public float ActionEffectValue { private get; set; }
        public float InjuryRecoveryTime { private get; set; }

        public AutoActionController(MotionEngine motionEngine, DisplayController displayController, StatusEventDispatcher statusEventDispatcher)
        {
            _motionEngine = motionEngine;
            _displayController = displayController;
            _statusEventDispatcher = statusEventDispatcher;

            _actionInProgress = false;
            _actionEffectHasFired = false;
            _autoActionDisabled = false;
            _actionTarget = null;
            _actionLocation = Vector3.zero;
            _cooldownRemaining = 0.0f;
            _actionBlockDuration = 0.0f;
        }

        private void SetAutoActionDisabledState(bool isDisabled)
        {
            _autoActionDisabled = isDisabled;

            if ((!isDisabled) && (_actionTarget != null) && (!CloseEnoughToTarget()))
            {
                _actionTarget = null;
                _displayController.CompleteAutoAction();
            }
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
                case StatusMessage.ReduceHealth: HandleCharacterHurt(target); break;
                case StatusMessage.CharacterDead: HandleCharacterDeath(originator); break;
            }
        }

        private void HandleCharacterHurt(Transform hurtCharacter)
        {
            if (hurtCharacter == Transform)
            {
                _actionBlockDuration = Mathf.Max(_actionBlockDuration, 0.5f);
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
            if ((_actionTarget != null) && (!_autoActionDisabled))
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

                    if ((_cooldownRemaining <= 0.0f) && (_actionBlockDuration <= 0.0f))
                    {
                        StartAutoActionSequence();
                    }
                }
                else if (!_actionInProgress)
                {
                    if (_actionBlockDuration > 0.0f)
                    {
                        _motionEngine.StopMoving();
                    }
                    else
                    {
                        _motionEngine.MoveTowardsPosition(_actionLocation);
                        _displayController.IsMoving = true;
                    }
                }
            }

            HandleTimerUpdates();
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

        private void HandleTimerUpdates()
        {
            if ((_actionBlockDuration > 0.0f) && (_actionBlockDuration - Time.deltaTime <= 0.0f) && (_actionInProgress) && (_actionEffectHasFired))
            {
                CompleteAutoAction();
            }

            _actionBlockDuration = Mathf.Max(_actionBlockDuration - Time.deltaTime, 0.0f);
            _cooldownRemaining = Mathf.Max(_cooldownRemaining - Time.deltaTime, 0.0f);
        }

        private void StartAutoActionSequence()
        {
            _actionInProgress = true;
            _actionEffectHasFired = false;
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
            _actionEffectHasFired = true;
            _statusEventDispatcher.FireStatusEvent(_actionTarget, ActionInvokationStatusEvent, ActionEffectValue);
        }

        private void CompleteAutoAction()
        {
            _actionInProgress = false;
            _statusEventDispatcher.FireStatusEvent(StatusMessage.CompletedAutoAction);
            _displayController.CompleteAutoAction();
            _cooldownRemaining = Cooldown;
        }

        public void AssignTarget(Transform target)
        {
            Debug.Log(Transform.name + " was assigned " + target.name + " as a target");

            _autoActionDisabled = false;
            _actionTarget = target;
        }
    }
}
