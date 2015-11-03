using UnityEngine;

namespace Scripts.Generic
{
    public class HealthBar : MonoBehaviour
    {
        private Transform _barTransform;
        private Transform _parentTransform;

        private void Awake()
        {
            _barTransform = transform.FindChild("Health Bar");
            _parentTransform = transform.parent;
        }

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void HandleStatusEvent(Transform source, Transform target, StatusEvent message, float value)
        {
            if ((target == _parentTransform) && (message == StatusEvent.SetHealthBarValue))
            {
                if (value <= 0.0f)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    _barTransform.localScale = new Vector3(value, 1.0f, 1.0f);
                }
            }
        }
    }
}
