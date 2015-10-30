﻿using UnityEngine;

namespace Scripts.Generic
{
    public class StatusEventDispatcher
    {
        public delegate void HandleStatusEvent(Transform Source, Transform target, string message, float value);
        public static event HandleStatusEvent StatusEventHandler;

        public Transform Source { private get; set; }

        public void FireStatusEvent(Transform target, string message, float value)
        {
            if (StatusEventHandler != null) { StatusEventHandler(Source, target, message, value); }
        }
    }
}
