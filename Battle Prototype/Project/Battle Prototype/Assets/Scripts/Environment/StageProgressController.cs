using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.Data_Models;

namespace Scripts.Environment
{
    public class StageProgressController : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody2D _rigidBody2D;
        private int _movementDirection;
        public float _previousXPosition;

        private RangeAttribute _activeLimits;

        private FieldBoundaryEventDispatcher _fieldLimitEventDispatcher;

        private StageProgressController() : base()
        {
            _fieldLimitEventDispatcher = new FieldBoundaryEventDispatcher();

            InitializeStageMovementRanges();
        }

        private void OnEnable()
        {
            UIEventDispatcher.ButtonEventHandler += HandleButtonEvent;
        }

        private void OnDisable()
        {
            UIEventDispatcher.ButtonEventHandler -= HandleButtonEvent;
        }

        private void HandleButtonEvent(string buttonName, bool isPressed)
        {
            _movementDirection = 0;

            if (isPressed)
            {
                switch (buttonName)
                {
                    case "MoveLeftButton": _movementDirection = -1; break;
                    case "MoveRightButton": _movementDirection = 1; break;
                }
            }
        }

        private void Awake()
        {
            _transform = transform;
            _rigidBody2D = _transform.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if ((_movementDirection != 0) && (_previousXPosition != _transform.position.x))
            {
                EnforceMovementLimits();
            }

            _rigidBody2D.velocity = new Vector2(Stage_Movement_Speed * _movementDirection, 0.0f);
            _previousXPosition = _transform.position.x;
        }

        private void EnforceMovementLimits()
        {
            if (_movementDirection < 0)
            {
                if ((_previousXPosition > _activeLimits.min) && (_transform.position.x <= _activeLimits.min))
                {
                    _movementDirection = 0;
                    _fieldLimitEventDispatcher.FireFieldBoundaryUpdateEvent(Direction.Left, true);
                }
                else if ((_previousXPosition >= _activeLimits.max) && (_transform.position.x < _activeLimits.max))
                {
                    _fieldLimitEventDispatcher.FireFieldBoundaryUpdateEvent(Direction.Right, false);
                }
            }
            else if (_movementDirection > 0)
            {
                if ((_previousXPosition < _activeLimits.max) && (_transform.position.x >= _activeLimits.max))
                {
                    _movementDirection = 0;
                    _fieldLimitEventDispatcher.FireFieldBoundaryUpdateEvent(Direction.Right, true);
                }
                else if ((_previousXPosition <= _activeLimits.min) && (_transform.position.x > _activeLimits.min))
                {
                    _fieldLimitEventDispatcher.FireFieldBoundaryUpdateEvent(Direction.Left, false);
                }
            }
        }

        private void InitializeStageMovementRanges()
        {
            _activeLimits = new RangeAttribute(0.0f, 5.0f);
        }

        public const float Stage_Movement_Speed = 1.0f;
    }
}
