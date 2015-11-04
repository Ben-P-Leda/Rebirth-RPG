using UnityEngine;

namespace Scripts.Generic
{
    public class CharacterStats
    {
        private float _currentHealth;
        private float _autoActionCooldown;

        public float InitialHealth { get; private set; }
        public float Speed { get; private set; }
        public float TimeBetweenAutoActions { get; private set; }

        public float MovementOffsetFromActionTarget { get; private set; }
        public float AutoActionHorizontalRange { get; private set; }
        public float AutoActionVerticalRange { get; private set; }

        public bool ReadyForAutoAction { get { return _autoActionCooldown <= 0.0f; } }
        public float HealthFraction { get { return _currentHealth / InitialHealth; } }
        public bool IsDead { get { return _currentHealth <= 0.0f; } }

        public void CreateForRole(string tag)
        {
            switch (tag.Substring(tag.IndexOf("-") + 1))
            {
                case "Fighter": SetForTank(); break;
                case "Healer": SetForCaster(); break;
            }

            _currentHealth = InitialHealth;
            _autoActionCooldown = 0.0f;
        }

        public void ResetAutoActionCooldown()
        {
            _autoActionCooldown = TimeBetweenAutoActions / Speed;
        }

        public void UpdateHealth(float delta)
        {
            _currentHealth -= delta;
        }

        public void UpdateCooldownTimer(float delta)
        {
            _autoActionCooldown = Mathf.Max(_autoActionCooldown - Time.deltaTime, 0.0f);
        }

        private void SetForTank()
        {
            MovementOffsetFromActionTarget = 0.75f;
            AutoActionHorizontalRange = 0.05f;
            AutoActionVerticalRange = 0.05f;

            Speed = 1.0f;
            TimeBetweenAutoActions = 2.0f;
            InitialHealth = 60.0f;
        }

        public void SetForCaster()
        {
            MovementOffsetFromActionTarget = 0.0f;
            AutoActionHorizontalRange = 3.0f;
            AutoActionVerticalRange = 2.0f;

            Speed = 1.0f;
            TimeBetweenAutoActions = 2.0f;
            InitialHealth = 30.0f;
        }
    }
}
