using UnityEngine;
using Scripts.All_Characters;
using Scripts.Event_Dispatchers;
using Scripts.UI;
using Scripts.Environment;

namespace Scripts.Player_Characters
{
    public class StageMotionEngine
    {
        private DisplayController _displayController;

        private Transform _transform;
        private Rigidbody2D _rigidbody2D;

        public Transform Transform { set { _transform = value; _rigidbody2D = value.GetComponent<Rigidbody2D>(); } }

        public StageMotionEngine(DisplayController displayController)
        {
            _displayController = displayController;
        }

        public void WireUpEventHandlers()
        {
            UIEventDispatcher.ButtonEventHandler += HandleButtonEvent;
        }

        public void UnhookEventHandlers()
        {
            UIEventDispatcher.ButtonEventHandler -= HandleButtonEvent;
        }

        private void HandleButtonEvent(string buttonName, bool isPressed)
        {
            if (SceneMovementButton.Movement_Buttons.Contains(buttonName))
            {
                if (isPressed)
                {
                    StartMovement(buttonName.Contains("Left") ? -1 : 1);
                }
                else
                {
                    StopMovement();
                }
            }
        }

        private void StartMovement(int direction)
        {
            _rigidbody2D.velocity = new Vector2(StageProgressController.Stage_Movement_Speed * direction, 0.0f);
            _displayController.IsMoving = true;
            _displayController.SetFacing(_transform.position + new Vector3(direction, 0.0f, 0.0f));
        }

        private void StopMovement()
        {
            _rigidbody2D.velocity = Vector3.zero;
            _displayController.IsMoving = false;
        }
    }
}
