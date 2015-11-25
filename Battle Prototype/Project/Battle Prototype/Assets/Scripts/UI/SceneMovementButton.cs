using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.UI;
using Scripts.Data_Models;

namespace Scripts.UI
{
    public class SceneMovementButton : MonoBehaviour
    {
        private Rect _displayArea;
        private bool _held;
        private bool _wasHeld;
        private string _buttonName;
        private bool _disabled;

        private UIEventDispatcher _uiEventDispatcher;

        public Direction MovementDirection;
        public Texture2D Texture;

        public SceneMovementButton()
        {
            _held = false;
            _wasHeld = false;
            _buttonName = "";
            _disabled = false;

            _uiEventDispatcher = new UIEventDispatcher();
        }

        private void OnEnable()
        {
            FieldBoundaryEventDispatcher.FieldBoundaryUpdateEventHandler += HandleBoundaryEvent;
        }

        private void OnDisable()
        {
            FieldBoundaryEventDispatcher.FieldBoundaryUpdateEventHandler -= HandleBoundaryEvent;
        }

        private void HandleBoundaryEvent(Direction boundaryDirection, bool atBoundary)
        {
            if (boundaryDirection == MovementDirection)
            {
                _disabled = atBoundary;
            }
        }

        private void Awake()
        {
            _buttonName = transform.name;
        }

        private void Start()
        {
            float offset = (Screen.width - (Texture.width * UIUtilities.Scaling)) * 0.5f;

            _displayArea = new Rect(
                offset + (offset * (int)MovementDirection),
                Screen.height - (Texture.height * UIUtilities.Scaling),
                Texture.width * UIUtilities.Scaling,
                Texture.height * UIUtilities.Scaling);
        }

        private void OnGUI()
        {
            if (!_disabled)
            {
                if ((GUI.RepeatButton(_displayArea, Texture)) && (!_held))
                {
                    _held = true;

                    if (!_wasHeld)
                    {
                        _uiEventDispatcher.FireUIButtonEvent(_buttonName, true);
                    }
                }
            }
        }

        private void Update()
        {
            if ((_wasHeld) && (!_held))
            {
                _uiEventDispatcher.FireUIButtonEvent(_buttonName, false);
            }

            _wasHeld = _held;
            _held = false;
        }

        public const string Movement_Buttons = "MoveLeftButton,MoveRightButton";
    }
}