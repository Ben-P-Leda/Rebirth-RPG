using UnityEngine;

using Scripts.Event_Dispatchers;

namespace Scripts.Player_Characters
{
    public class SelectionHandler
    {
        private StatusEventDispatcher _statusEventDispatcher;
        private Transform _currentActiveCharacter;

        public Transform Transform { private get; set; }

        public SelectionHandler(StatusEventDispatcher statusEventDispatcher)
        {
            _statusEventDispatcher = statusEventDispatcher;
            _currentActiveCharacter = null;
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
                case StatusMessage.CharacterSelected: HandleCharacterSelection(target); break;
                case StatusMessage.CharacterActivated: SetActiveCharacter(target); break;
                case StatusMessage.CharacterDeactivated: SetActiveCharacter(null); break;
            }
        }

        private void HandleCharacterSelection(Transform selection)
        {
            if (selection == Transform)
            {
                Debug.Log(Transform.name + " handle self");
                HandleSelfSelection();
            }
            else if (_currentActiveCharacter == Transform)
            {
                Debug.Log(Transform.name + " handle other when active " + selection.name);
                HandleOtherSelectionWhenActive(selection);
            }
        }

        private void HandleSelfSelection()
        {
            if (CharacterUtilities.CharacterTargetsAllies(_currentActiveCharacter))
            {
                Debug.Log(_currentActiveCharacter.name + ": targets FRIENDLIES - setting target to " + Transform.name);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.AlliedActionTargetSelected, 0.0f);
                _statusEventDispatcher.FireStatusEvent(_currentActiveCharacter, StatusMessage.CharacterDeactivated, 0.0f);
            }
            else
            {
                Debug.Log(Transform.name + " was set to active");
                _currentActiveCharacter = Transform;
                _statusEventDispatcher.FireStatusEvent(Transform, StatusMessage.CharacterActivated, 0.0f);
            }
        }

        private void HandleOtherSelectionWhenActive(Transform selection)
        {
            if ((!CharacterUtilities.CharacterTargetsAllies(Transform)) && (!CharacterUtilities.CharactersAreAllies(Transform, selection)))
            {
                Debug.Log(Transform.name + ": targets HOSTILES - setting target to " + selection.name);
                _statusEventDispatcher.FireStatusEvent(selection, StatusMessage.EnemyActionTargetSelected, 0.0f);
                _statusEventDispatcher.FireStatusEvent(selection, StatusMessage.CharacterDeactivated, 0.0f);
            }
        }

        private void SetActiveCharacter(Transform activeCharacter)
        {
            _currentActiveCharacter = activeCharacter;
            Debug.Log(Transform.name + (activeCharacter == null ? " cleared current character selection" : " logged as active by " + activeCharacter.name));
        }
    }
}