using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.UI;

namespace Scripts.UI
{
    public class SceneMovementButton : MonoBehaviour
    {
        private Rect _displayArea;
        private bool _held;
        private bool _wasHeld;
        private string _buttonName;

        private UIEventDispatcher _uiEventDispatcher;

        public int MovementStep;
        public Texture2D Texture;

        public SceneMovementButton()
        {
            _held = false;
            _wasHeld = false;
            _buttonName = "";

            _uiEventDispatcher = new UIEventDispatcher();
        }

        private void Awake()
        {
            _buttonName = transform.name;
        }

        private void Start()
        {
            float offset = (Screen.width - (Texture.width * UIUtilities.Scaling)) * 0.5f;

            _displayArea = new Rect(
                offset + (offset * MovementStep),
                Screen.height - (Texture.height * UIUtilities.Scaling),
                Texture.width * UIUtilities.Scaling,
                Texture.height * UIUtilities.Scaling);
        }

        private void OnGUI()
        {
            if ((GUI.RepeatButton(_displayArea, Texture)) && (!_held))
            {
                _held = true;

                if (!_wasHeld)
                {
                    Debug.Log(string.Format("Button {0} PRESSED", _buttonName));
                    _uiEventDispatcher.FireUIButtonEvent(_buttonName, true);
                }
            }
        }

        private void Update()
        {
            if ((_wasHeld) && (!_held))
            {
                Debug.Log(string.Format("Button {0} RELEASED", _buttonName));
                _uiEventDispatcher.FireUIButtonEvent(_buttonName, false);
            }

            _wasHeld = _held;
            _held = false;
        }

        public const string Movement_Buttons = "MoveLeftButton,MoveRightButton";
    }
}