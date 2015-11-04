using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class FieldMovementController
    {
        private DisplayController _displayController;

        private Transform _transform;
        private bool _isActive;
        private bool _isMoving;
        private Vector3 _movementTarget;

        public Transform Transform { set { _transform = value; _movementTarget = value.position; } }
        public Rigidbody2D Rigidbody2D { private get; set; }

        public float MovementSpeed { private get; set; }

        public FieldMovementController(DisplayController displayController)
        {
            _displayController = displayController;

            _isActive = false;
            _isMoving = false;
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
            if (_isMoving)
            {
                Vector2 vectorToTarget = _movementTarget - _transform.position;

                if (vectorToTarget.magnitude > Movement_Target_Stopping_Distance)
                {
                    Rigidbody2D.velocity = vectorToTarget.normalized * MovementSpeed;
                    _displayController.SetFacing(_movementTarget);
                }
                else
                {
                    Rigidbody2D.velocity = Vector3.zero;
                    _isMoving = false;
                }
            }
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.CharacterActivated: _isActive = (_transform == target); break;
                case StatusMessage.CharacterDeactivated: _isActive = false; break;
            }
        }

        private void HandleFieldClick(Vector3 clickLocation)
        {
            if (_isActive)
            {
                _movementTarget = new Vector3(clickLocation.x, clickLocation.y, _transform.position.z);
                _isMoving = true;
            }
        }

        private const float Movement_Target_Stopping_Distance = 0.05f;
    }
}
