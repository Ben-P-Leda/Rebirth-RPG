using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class PlayerCharacterController : MonoBehaviour
    {
        private Transform _transform;

        private StatusEventDispatcher _statusEventDispatcher;
        private DisplayController _displayController;

        private SelectionHandler _selectionHandler;
        private FieldMovementController _fieldMovementController;

        public PlayerCharacterController() : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _displayController = new DisplayController();

            _selectionHandler = new SelectionHandler(_statusEventDispatcher);

            _fieldMovementController = new FieldMovementController(_displayController);
            _fieldMovementController.MovementSpeed = 1.0f;
        }

        private void OnEnable()
        {
            _selectionHandler.WireUpEventHandlers();
            _fieldMovementController.WireUpEventHandlers();
        }

        private void OnDisable()
        {
            _selectionHandler.UnhookEventHandlers();
            _fieldMovementController.UnhookEventHandlers();
        }

        private void Awake()
        {
            _transform = transform;

            _statusEventDispatcher.Source = _transform;
            _displayController.Transform = _transform;
            _selectionHandler.Transform = _transform;
            _fieldMovementController.Transform = _transform;
            _fieldMovementController.Rigidbody2D = _transform.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _fieldMovementController.Update();
        }

        public void HandleClickedOn()
        {
            _statusEventDispatcher.FireStatusEvent(_transform, StatusMessage.CharacterSelected, 0.0f);
        }
    }
}
