using UnityEngine;
using Scripts.Generic;

namespace Scripts.Player_Characters
{
    public class SelectionMarkerToggle : MonoBehaviour
    {
        private Transform _parentTransform;
        private SpriteRenderer _renderer;

        private void OnEnable()
        {
            SelectionController.SelectionStateHandler += HandleSelectionChange;
            StatusEventDispatcher.StatusEventHandler += HandleStatusChange; ;
        }

        private void OnDisable()
        {
            SelectionController.SelectionStateHandler -= HandleSelectionChange;
            StatusEventDispatcher.StatusEventHandler -= HandleStatusChange; ;
        }

        private void Awake()
        {
            _parentTransform = transform.parent;
            _renderer = transform.GetComponent<SpriteRenderer>();
        }

        private void HandleSelectionChange(Transform targetTransform)
        {
            _renderer.enabled = (_parentTransform == targetTransform);
        }

        private void HandleStatusChange(Transform source, Transform target, StatusEvent message, float value)
        {
            if ((message == StatusEvent.Death) && (source == _parentTransform))
            {
                gameObject.SetActive(false);
            }
        }
    }
}