using UnityEngine;
using Scripts.Data_Models;

namespace Scripts.Event_Dispatchers
{
    public class FieldBoundaryEventDispatcher
    {
        public delegate void HandleFieldBoundaryUpdateEvent(Direction side, bool atBoundary);
        public static event HandleFieldBoundaryUpdateEvent FieldBoundaryUpdateEventHandler;

        public void FireFieldBoundaryUpdateEvent(Direction side, bool atBoundary)
        {
            Debug.Log("BOUNDARY EVENT: " + side.ToString() + " " + atBoundary.ToString());

            if (FieldBoundaryUpdateEventHandler != null) { FieldBoundaryUpdateEventHandler(side, atBoundary); }
        }
    }
}
