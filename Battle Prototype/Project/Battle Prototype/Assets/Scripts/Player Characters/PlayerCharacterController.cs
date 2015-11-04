using UnityEngine;
using Scripts.All_Characters;

namespace Scripts.Player_Characters
{
    public class PlayerCharacterController : MonoBehaviour
    {
        private DisplayController _displayController;
        private FieldMovementController _fieldMovementController;

        public PlayerCharacterController() : base()
        {
            _displayController = new DisplayController();
            _fieldMovementController = new FieldMovementController(_displayController);
        }

        private void OnEnable()
        {
            _fieldMovementController.WireUpEventHandlers();
        }

        private void OnDisable()
        {
            _fieldMovementController.UnhookEventHandlers();
        }

        private void Awake()
        {
            _displayController.Transform = transform;

            _fieldMovementController.Transform = transform;
            _fieldMovementController.Rigidbody2D = transform.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _fieldMovementController.Update();
        }
    }
}
