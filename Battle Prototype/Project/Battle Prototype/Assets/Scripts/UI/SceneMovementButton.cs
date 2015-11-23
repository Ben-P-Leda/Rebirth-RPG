using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.UI;

namespace Scripts.UI
{
    public class SceneMovementButton : MonoBehaviour
    {
        private Rect _displayArea;
        private bool _held;
        private string _buttonName;

        public int MovementStep;
        public Texture2D Texture;

        public UIEventDispatcher UIEventDispatcher { private get; set; }

        public SceneMovementButton()
        {
            _held = false;
            _buttonName = "";

            UIEventDispatcher = null;
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
            bool wasHeld = _held;
            _held = GUI.RepeatButton(_displayArea, Texture);

            if (wasHeld != _held)
            {
                Debug.Log(string.Format("Button {0} held state updated to {1}", _buttonName, _held));
                if (UIEventDispatcher != null)
                {
                    UIEventDispatcher.FireUIButtonEvent(_buttonName, _held);
                }
            }
        }
    }
}