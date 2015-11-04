using UnityEngine;

using Scripts.Event_Dispatchers;

namespace Scripts.Player_Characters
{
    public class SelectionHandler : MonoBehaviour
    {
        private Transform _transform;
        private StatusEventDispatcher _statusEventDispatcher;
        private Transform _currentActiveCharacter;

        private SelectionHandler() : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _currentActiveCharacter = null;
        }

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void Awake()
        {
            _transform = transform;
            _statusEventDispatcher.Source = transform;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            Debug.Log(_transform.name + " got event " + message.ToString() + " from " + originator.name);

            if ((message == StatusMessage.CharacterSelected) && (target == _transform))
            {
                if (ActiveCharacterAwaitingAlliedTarget())
                {
                    Debug.Log(_currentActiveCharacter.name + ": setting target to " + _transform.name);
                    _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.ActionTargetSelected, 0.0f);
                }
                else
                {
                    CheckForActivation(target);
                }
            }

            if ((message == StatusMessage.CharacterActivated) && (target != _transform))
            {
                Debug.Log(_transform.name + " logged " + target.name + " as the active character");

                _currentActiveCharacter = target;
            }

            if (message == StatusMessage.ActionTargetSelected)
            {
                _currentActiveCharacter = null;
                Debug.Log(_transform.name + " cleared current character selection");
            }
        }

        private bool ActiveCharacterAwaitingAlliedTarget()
        {
            bool currentCharacterTargetsAllies = false;

            if ((_currentActiveCharacter != null)  && (_currentActiveCharacter.tag.ToLower().Contains("healer")))
            {
                currentCharacterTargetsAllies = true;
            }

            if (currentCharacterTargetsAllies)
                Debug.Log("CURRENT SELECTION TARGETS ALLIES! (" + _transform.name + ")");

            return currentCharacterTargetsAllies;
        }

        private void CheckForActivation(Transform selectedCharacterTransform)
        {
            Debug.Log(_transform.name + " was set to active");

            _currentActiveCharacter = _transform;
            _statusEventDispatcher.FireStatusEvent(_transform, StatusMessage.CharacterActivated, 0.0f);
        }
    }
}