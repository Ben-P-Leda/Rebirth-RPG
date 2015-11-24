using UnityEngine;
using Scripts.Event_Dispatchers;

namespace Scripts.Environment
{
    public class StageProgressController : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody2D _rigidBody2D;
        private int _movementDirection;

        private void Awake()
        {
            _transform = transform;
            _rigidBody2D = _transform.GetComponent<Rigidbody2D>();
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

        private void FixedUpdate()
        {
            _rigidBody2D.velocity = new Vector2(Stage_Movement_Speed * _movementDirection, 0.0f);
        }

        public const float Stage_Movement_Speed = 1.0f;
    }
}
