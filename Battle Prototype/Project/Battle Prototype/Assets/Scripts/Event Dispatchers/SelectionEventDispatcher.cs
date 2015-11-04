using UnityEngine;

namespace Scripts.Event_Dispatchers
{
    public class SelectionEventDispatcher : MonoBehaviour
    {
        private Transform _transform;

        private StatusEventDispatcher _statusEventDispatcher;

        public SelectionEventDispatcher() : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
        }

        private void Awake()
        {
            _transform = transform;
            _statusEventDispatcher.Source = _transform;
        }

        public void HandleClickedOn()
        {
            _statusEventDispatcher.FireStatusEvent(_transform, StatusMessage.CharacterSelected, 0.0f);
        }
    }
}