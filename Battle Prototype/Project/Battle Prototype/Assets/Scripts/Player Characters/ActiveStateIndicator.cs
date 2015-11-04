using UnityEngine;
using Scripts.Event_Dispatchers;

namespace Scripts.Player_Characters
{
    public class ActiveStateIndicator : MonoBehaviour
    {
        private Transform _parentTransform;
        private SpriteRenderer _renderer;

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void Awake()
        {
            _parentTransform = transform.parent;
            _renderer = transform.GetComponent<SpriteRenderer>();
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.CharacterActivated: _renderer.enabled = (_parentTransform == target); break;
                case StatusMessage.CharacterDeactivated: _renderer.enabled = false; break;
            }
        }
    }
}