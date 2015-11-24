using UnityEngine;

namespace Scripts.Event_Dispatchers
{
    public class FieldLimitEventDispatcher
    {
        public delegate void HandleFieldLimitUpdateEvent(Side side, float limit);
        public static event HandleFieldLimitUpdateEvent FieldLimitUpdateEventHandler;

        private void FireFieldLimitUpdateEvent(Side side, float limit)
        {
            if (FieldLimitUpdateEventHandler != null) { FieldLimitUpdateEventHandler(side, limit); }
        }
    }

    public enum Side
    {
        Left,
        Right
    }
}
