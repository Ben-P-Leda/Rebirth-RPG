using UnityEngine;
using Scripts.Event_Dispatchers;

namespace Scripts.Environment
{
    public class StageProgressController : MonoBehaviour
    {
        private Transform _transform;
        private int _movementDirection;

        private void Awake()
        {
            _transform = transform;
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
        }
    }
}
