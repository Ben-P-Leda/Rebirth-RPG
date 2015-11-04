using UnityEngine;

namespace Scripts.Generic
{
    public class ActionTargetResolver
    {
        private string _faction;
        private bool _targetOwnFaction;

        public ActionTargetResolver()
        {
        }

        public void InitializeTargetType(Transform sourceCharacterTransform)
        {
            _faction = sourceCharacterTransform.tag.Substring(0, sourceCharacterTransform.tag.IndexOf("-"));
            _targetOwnFaction = sourceCharacterTransform.tag.Contains("Healer");
        }

        public bool IsValidTarget(Transform selectedTransform)
        {
            string targetFaction = selectedTransform.tag.Substring(0, selectedTransform.tag.IndexOf("-"));

            if ((targetFaction == _faction) && (_targetOwnFaction)) { return true; }
            if ((targetFaction != _faction) && (!_targetOwnFaction)) { return true; }

            return false;
        }
    }
}
