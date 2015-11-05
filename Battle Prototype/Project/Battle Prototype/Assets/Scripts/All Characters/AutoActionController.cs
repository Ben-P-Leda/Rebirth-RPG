using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.All_Characters
{
    public class AutoActionController
    {
        private MotionEngine _motionEngine;
        private DisplayController _displayController;

        private bool _isActive;
        private bool _actionInProgress;
        private bool _fieldMovementInProgress;
        private Transform _actionTarget;
        private Vector3 _actionLocation;

        public Transform Transform { private get; set; }
        public Rigidbody2D Rigidbody2D { private get; set; }
        public float ActionLocationOffset { private get; set; }
        public Vector2 RequiredTargetProximity { private get; set; }

        public AutoActionController(MotionEngine motionEngine, DisplayController displayController)
        {
            _motionEngine = motionEngine;
            _displayController = displayController;

            _isActive = false;
            _actionInProgress = false;
            _fieldMovementInProgress = false;
            _actionTarget = null;
            _actionLocation = Vector3.zero;

            ActionLocationOffset = 0;
            RequiredTargetProximity = new Vector2(Default_Required_Proximity, Default_Required_Proximity);
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
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
            }
        }

        private void HandleFieldMovementStart()
        {
            if (_isActive)
            {
                _fieldMovementInProgress = true;

                // TODO: Pause auto action as we are moving towards a point on the field
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
                }
                else if (!_actionInProgress)
                {
                    _motionEngine.MoveTowardsPosition(_actionLocation);
                }
            }
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


        private const float Default_Required_Proximity = 0.05f;
    }
}
