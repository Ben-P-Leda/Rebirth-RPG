using UnityEngine;
using Scripts.Generic;

namespace Scripts.Player_Characters
{
    public class SelectionMarkerToggle : MonoBehaviour
    {
        private Transform _parentTransform;
        private SpriteRenderer _renderer;
        private bool _nextSelectionSelectsFriendlyTarget;
        private string _faction;

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

            _nextSelectionSelectsFriendlyTarget = false;
            _faction = _parentTransform.tag.Substring(0, _parentTransform.tag.IndexOf("-"));
        }

        private void HandleSelectionChange(Transform targetTransform)
        {
            if (_nextSelectionSelectsFriendlyTarget)
            {
                _renderer.enabled = false;
                _nextSelectionSelectsFriendlyTarget = false;
            }
            else
            {
                _renderer.enabled = (_parentTransform == targetTransform);
                _nextSelectionSelectsFriendlyTarget = (targetTransform.tag == _faction + "-Healer");
            }
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