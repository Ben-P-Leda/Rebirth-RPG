using UnityEngine;

namespace Scripts.Event_Dispatchers
{
    public class UIEventDispatcher
    {
        public delegate void HandleButtonEvent(string buttonName, bool isPressed);
        public static event HandleButtonEvent ButtonEventHandler;

        public void FireUIButtonEvent(string buttonName, bool isPressed)
        {
            if (ButtonEventHandler != null)
            {
                ButtonEventHandler(buttonName, isPressed);
            }
        }
    }
}
