using UnityEngine;
using Scripts.Event_Dispatchers;

namespace Scripts.All_Characters
{
    public class HealthManager
    {
        private StatusEventDispatcher _statusEventDispatcher;

        private Transform _transform;
        private Transform _barTransform;
        private float _currentHealth;
        private float _maximumHealth;

        public float MaximumHealth { set { _maximumHealth = value; _currentHealth = value; } }

        public Transform Transform { set { _transform = value; _barTransform = value.FindChild("Health Bar Container").FindChild("Health Bar"); } }

        public HealthManager(StatusEventDispatcher statusEventDispatcher)
        {
            _statusEventDispatcher = statusEventDispatcher;
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            if (target == _transform)
            {
                Debug.Log(_transform.name + " HM got event " + message.ToString());

                switch (message)
                {
                    case StatusMessage.ReduceHealth: HandleHealthLoss(value); break;
                    case StatusMessage.IncreaseHealth: HandleHealthGain(value); break;
                }
            }
        }

        private void HandleHealthLoss(float delta)
        {
            _currentHealth = Mathf.Max(0.0f, _currentHealth - delta);
            UpdateHealthBar();
        }

        private void HandleHealthGain(float delta)
        {
            _currentHealth = Mathf.Min(_maximumHealth, _currentHealth - delta);
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            Debug.Log(_currentHealth.ToString() + "/" + _maximumHealth.ToString());

            _barTransform.localScale = new Vector3(_currentHealth / _maximumHealth, 1.0f, 1.0f);
        }
    }
}
