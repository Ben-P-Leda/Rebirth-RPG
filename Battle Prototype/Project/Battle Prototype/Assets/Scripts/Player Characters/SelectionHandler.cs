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
            switch (message)
            {
                case StatusMessage.CharacterSelected: HandleCharacterSelection(target); break;
                case StatusMessage.CharacterActivated: SetActiveCharacter(target); break;
                case StatusMessage.CharacterDeactivated: SetActiveCharacter(null); break;
            }
        }

        private void HandleCharacterSelection(Transform selection)
        {
            if (selection == _transform)
            {
                Debug.Log(_transform.name + " handle self");
                HandleSelfSelection();
            }
            else if (_currentActiveCharacter == _transform)
            {
                Debug.Log(_transform.name + " handle other when active " + selection.name);
                HandleOtherSelectionWhenActive(selection);
            }
        }

        private void HandleSelfSelection()
        {
            if (CharacterUtilities.CharacterTargetsAllies(_currentActiveCharacter))
            {
                Debug.Log(_currentActiveCharacter.name + ": targets FRIENDLIES - setting target to " + _transform.name);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.AlliedActionTargetSelected, 0.0f);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.CharacterDeactivated, 0.0f);
            }
            else
            {
                Debug.Log(_transform.name + " was set to active");
                _currentActiveCharacter = _transform;
                _statusEventDispatcher.FireStatusEvent(_transform, StatusMessage.CharacterActivated, 0.0f);
            }
        }

        private void HandleOtherSelectionWhenActive(Transform selection)
        {
            if ((!CharacterUtilities.CharacterTargetsAllies(_transform)) && (!CharacterUtilities.CharactersAreAllies(_transform, selection)))
            {
                Debug.Log(_transform.name + ": targets HOSTILES - setting target to " + selection.name);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.EnemyActionTargetSelected, 0.0f);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.CharacterDeactivated, 0.0f);
            }
        }

        private void SetActiveCharacter(Transform activeCharacter)
        {
            _currentActiveCharacter = activeCharacter;
            Debug.Log(_transform.name + (activeCharacter == null ? " cleared current character selection" : " logged as active by " + activeCharacter.name));
        }
    }
}