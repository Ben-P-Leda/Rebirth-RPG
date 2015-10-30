using UnityEngine;

namespace Scripts.Generic
{
    public class SelectionController : MonoBehaviour
    {
        public delegate void UpdateSelectionState(Transform clickTargetTransform);
        public static event UpdateSelectionState SelectionStateHandler;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void HandleClickedOn()
        {
            if (SelectionStateHandler != null) { SelectionStateHandler(_transform); }
        }
    }
}