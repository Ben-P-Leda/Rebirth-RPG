using UnityEngine;

namespace Scripts.Event_Dispatchers
{
    public class StatusEventDispatcher
    {
        public delegate void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value);
        public static event HandleStatusEvent StatusEventHandler;

        public Transform Source { private get; set; }

        public void FireStatusEvent(Transform target, StatusMessage message, float value)
        {
            if (StatusEventHandler != null) 
            { 
                StatusEventHandler(Source, target, message, value); 
            }
        }
    }

    public enum StatusMessage
    {
        CharacterSelected,
        CharacterActivated,
        CharacterDeactivated,
        AlliedActionTargetSelected,
        EnemyActionTargetSelected
    }
}
