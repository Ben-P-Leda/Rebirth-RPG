using UnityEngine;

namespace Scripts.Generic
{
    public class AnimationEventDispatcher : MonoBehaviour
    {
        public delegate void HandleAnimationEvent(Transform origin, AnimationEvent message);
        public static event HandleAnimationEvent AnimationEventHandler;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void FireAnimationEvent(AnimationEvent message)
        {
            if (AnimationEventHandler != null) { AnimationEventHandler(_transform.parent, message); }
        }

        public enum AnimationEvent
        {
            AutoActionEffectOccurs,
            AutoActionComplete
        }
    }
}
