using UnityEngine;
using System.Collections.Generic;
using Scripts.Event_Dispatchers;

namespace Scripts.Enemy_Characters
{
    public class ThreatReactionManager
    {
        private Transform _transform;
        private string _faction;
        private Dictionary<Transform, float> _threatValues;

        public Transform Transform { set { _transform = value; _faction = CharacterUtilities.GetFaction(value); } }

        public ThreatReactionManager()
        {
            _threatValues = new Dictionary<Transform, float>();
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
            switch (message)
            {
                case StatusMessage.ReduceHealth: UpdateThreatFromAttacker(target, originator, value); break;
                case StatusMessage.IncreaseHealth: UpdateThreatFromHealer(originator, value); break;
            }
        }

        private void UpdateThreatFromAttacker(Transform attackTarget, Transform attacker, float damageValue)
        {
            if (attackTarget == _transform)
            {
                UpdateThreat(attacker, damageValue);
            }
        }

        private void UpdateThreatFromHealer(Transform healer, float healingValue)
        {
            if (CharacterUtilities.GetFaction(healer) != _faction)
            {
                UpdateThreat(healer, healingValue);
            }
        }

        private void UpdateThreat(Transform threatSource, float threatValue)
        {
            Debug.Log(string.Format("{0} detected THREAT from {1} (value {2})", _transform.name, threatSource.name, threatValue));

            if (!_threatValues.ContainsKey(threatSource))
            {
                _threatValues.Add(threatSource, threatValue);
            }
            else
            {
                _threatValues[threatSource] += threatValue;
            }
        }
    }
}
