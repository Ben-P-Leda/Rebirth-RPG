using UnityEngine;
using Scripts.Event_Dispatchers;
using AnimationEvent = Scripts.Event_Dispatchers.AnimationEvent;

namespace Scripts.All_Characters
{
    public class HealthManager
    {
        private StatusEventDispatcher _statusEventDispatcher;
        private DisplayController _displayController;

        private Transform _transform;
        private Transform _barTransform;
        private float _currentHealth;
        private float _maximumHealth;

        public float MaximumHealth { set { _maximumHealth = value; _currentHealth = value; } }

        public Transform Transform { set { _transform = value; _barTransform = value.FindChild("Health Bar Container").FindChild("Health Bar"); } }

        public HealthManager(StatusEventDispatcher statusEventDispatcher, DisplayController displayController)
        {
            _statusEventDispatcher = statusEventDispatcher;
            _displayController = displayController;
        }

        public void WireUpEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
            AnimationEventDispatcher.AnimationEventHandler += HandleAnimationEvent;
        }

        public void UnhookEventHandlers()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
            AnimationEventDispatcher.AnimationEventHandler -= HandleAnimationEvent;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            if (target == _transform)
            {
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
            if (_currentHealth <= 0.0f)
            {
                _displayController.TriggerDeathAnimation();
                _statusEventDispatcher.FireStatusEvent(StatusMessage.CharacterDead);
            }
            else
            {
                _displayController.TriggerHurtAnimation();
            }
        }

        private void HandleHealthGain(float delta)
        {
            _currentHealth = Mathf.Min(_maximumHealth, _currentHealth + delta);
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            _barTransform.localScale = new Vector3(_currentHealth / _maximumHealth, 1.0f, 1.0f);
        }

        private void HandleAnimationEvent(Transform originator, AnimationEvent message)
        {
            if ((message == AnimationEvent.DeathSequenceComplete) && (originator == _transform))
            {
                // TODO: remove from play gracefully, update any states
                _transform.gameObject.SetActive(false);
            }
        }
    }
}
