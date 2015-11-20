using UnityEngine;
using System.Collections.Generic;
using Scripts.All_Characters;
using Scripts.Event_Dispatchers;

namespace Scripts.Enemy_Characters
{
    public class ThreatReactionManager
    {
        private Transform _transform;
        private string _faction;
        private Dictionary<Transform, float> _threatValues;
        private Transform _currentTarget;
        private AutoActionController _autoActionController;

        public float DirectAttackThreatModifier { private get; set; }
        public float PcHealingThreatModifier { private get; set; }

        public Transform Transform { set { _transform = value; _faction = CharacterUtilities.GetFaction(value); } }

        public ThreatReactionManager(AutoActionController autoActionController)
        {
            _threatValues = new Dictionary<Transform, float>();
            _currentTarget = null;

            _autoActionController = autoActionController;
        }

        public void WireUpEventHandlers()
        {
            EnemyCharacterTargetSelector.TargetAssignmentHandler += HandleTargetAssignmentResponse;
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        public void UnhookEventHandlers()
        {
            EnemyCharacterTargetSelector.TargetAssignmentHandler -= HandleTargetAssignmentResponse;
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void HandleTargetAssignmentResponse(Transform originator, Transform target)
        {
            if (originator == _transform)
            {
                _currentTarget = target;
            }
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
                UpdateThreat(attacker, damageValue * DirectAttackThreatModifier);
            }
        }

        private void UpdateThreatFromHealer(Transform healer, float healingValue)
        {
            if (CharacterUtilities.GetFaction(healer) != _faction)
            {
                UpdateThreat(healer, healingValue * PcHealingThreatModifier);
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

            Transform greatestThreat = GetGreatestThreat();
            if (greatestThreat != _currentTarget)
            {
                _currentTarget = greatestThreat;
                _autoActionController.AssignTarget(greatestThreat);

                Debug.Log(string.Format("{0} switched target to greatest threat {1}", _transform.name, _currentTarget.name));
            }
        }

        private Transform GetGreatestThreat()
        {
            Transform greatestThreat = null;
            float highestThreatValue = 0.0f;

            foreach (KeyValuePair<Transform, float> kvp in _threatValues)
            {
                if (kvp.Value > highestThreatValue)
                {
                    greatestThreat = kvp.Key;
                    highestThreatValue = kvp.Value;
                }
            }

            return greatestThreat;
        }
    }
}
