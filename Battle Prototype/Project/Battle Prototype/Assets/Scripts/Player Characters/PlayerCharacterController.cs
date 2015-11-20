using UnityEngine;
using Scripts;
using Scripts.Data_Models;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class PlayerCharacterController : MonoBehaviour
    {
        private Transform _transform;
        public bool _isActive;

        private StatusEventDispatcher _statusEventDispatcher;
        private MotionEngine _motionEngine;
        private DisplayController _displayController;

        private SelectionHandler _selectionHandler;
        private FieldMovementController _fieldMovementController;
        private AutoActionController _autoActionController;
        private HealthManager _healthManager;

        public PlayerCharacterController() : base()
        {
            _isActive = false;

            _statusEventDispatcher = new StatusEventDispatcher();
            _motionEngine = new MotionEngine();
            _displayController = new DisplayController();
            _healthManager = new HealthManager(_statusEventDispatcher, _displayController);
            _selectionHandler = new SelectionHandler(_statusEventDispatcher);
            _fieldMovementController = new FieldMovementController(_motionEngine, _displayController, _statusEventDispatcher);
            _autoActionController = new AutoActionController(_motionEngine, _displayController, _statusEventDispatcher);
        }

        private void OnEnable()
        {
            _selectionHandler.WireUpEventHandlers();
            _fieldMovementController.WireUpEventHandlers();
            _autoActionController.WireUpEventHandlers();
            _healthManager.WireUpEventHandlers();

            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            _selectionHandler.UnhookEventHandlers();
            _fieldMovementController.UnhookEventHandlers();
            _autoActionController.UnhookEventHandlers();
            _healthManager.UnhookEventHandlers();

            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.CharacterActivated: _isActive = (_transform == target); break;
                case StatusMessage.CharacterDeactivated: _isActive = false; break;
                case StatusMessage.StartedFieldMovement: HandleFieldMovementStart(); break;
                case StatusMessage.CompletedFieldMovement: HandleFieldMovementCompletion(target); break;
                case StatusMessage.EnemyActionTargetSelected: HandleActionTargetAssignment(target, originator); break;
                case StatusMessage.AlliedActionTargetSelected: HandleActionTargetAssignment(originator, target); break;
            }
        }

        private void HandleFieldMovementStart()
        {
            if (_isActive)
            {
                _autoActionController.AutoActionDisabled = true;
            }
        }

        private void HandleFieldMovementCompletion(Transform characterCompletingMovement)
        {
            if (characterCompletingMovement == _transform)
            {
                _autoActionController.AutoActionDisabled = false;
            }
        }

        private void HandleActionTargetAssignment(Transform target, Transform assignTo)
        {
            if (assignTo == _transform)
            {
                _autoActionController.AssignTarget(target);
            }
        }

        private void Awake()
        {
            _transform = transform;

            ConnectComponentsToCharacter();
            SetCharacterStatistics();
        }

        private void ConnectComponentsToCharacter()
        {
            _statusEventDispatcher.Source = _transform;
            _motionEngine.Transform = _transform;
            _displayController.Transform = _transform;
            _selectionHandler.Transform = _transform;
            _fieldMovementController.Transform = _transform;
            _autoActionController.Transform = _transform;
            _healthManager.Transform = _transform;
        }

        private void SetCharacterStatistics()
        {
            CharacterData stats = CharacterDataProvider.Instance.GetCharacterData(_transform);

            _motionEngine.MovementSpeed = stats.MovementSpeed;

            _healthManager.MaximumHealth = stats.Health;

            _autoActionController.ActionLocationOffset = stats.ActionLocationOffset;
            _autoActionController.RequiredTargetProximity = stats.RequiredTargetProximity;
            _autoActionController.Cooldown = stats.AutoActionCooldown;
            _autoActionController.ActionInvokationStatusEvent = stats.AutoActionEffect;
            _autoActionController.ActionEffectValue = stats.AutoActionEffectMagnitude;
        }

        private void Update()
        {
            _fieldMovementController.Update();
            _autoActionController.Update();
        }

        public void HandleClickedOn()
        {
            _statusEventDispatcher.FireStatusEvent(StatusMessage.CharacterSelected);
        }
    }
}
