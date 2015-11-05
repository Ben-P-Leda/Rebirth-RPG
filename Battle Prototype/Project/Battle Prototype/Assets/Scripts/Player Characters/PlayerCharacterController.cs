using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class PlayerCharacterController : MonoBehaviour
    {
        private Transform _transform;

        private StatusEventDispatcher _statusEventDispatcher;
        private MotionEngine _motionEngine;
        private DisplayController _displayController;

        private SelectionHandler _selectionHandler;
        private FieldMovementController _fieldMovementController;
        private AutoActionController _autoActionController;
        private HealthManager _healthManager;

        public PlayerCharacterController() : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();

            _motionEngine = new MotionEngine();
            _motionEngine.MovementSpeed = 1.0f;
            _displayController = new DisplayController();

            _healthManager = new HealthManager(_statusEventDispatcher);
            _healthManager.MaximumHealth = 10.0f;

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
        }

        private void OnDisable()
        {
            _selectionHandler.UnhookEventHandlers();
            _fieldMovementController.UnhookEventHandlers();
            _autoActionController.UnhookEventHandlers();
            _healthManager.UnhookEventHandlers();
        }

        private void Awake()
        {
            _transform = transform;

            _statusEventDispatcher.Source = _transform;
            _motionEngine.Transform = _transform;
            _displayController.Transform = _transform;
            _selectionHandler.Transform = _transform;
            _fieldMovementController.Transform = _transform;
            _autoActionController.Transform = _transform;
            _healthManager.Transform = _transform;
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
