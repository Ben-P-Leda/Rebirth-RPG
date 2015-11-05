using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Enemy_Characters
{
    public class EnemyCharacterController : MonoBehaviour
    {
        private Transform _transform;

        private StatusEventDispatcher _statusEventDispatcher;
        private DisplayController _displayController;
        private HealthManager _healthManager;

        public EnemyCharacterController()
            : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _displayController = new DisplayController();

            _healthManager = new HealthManager(_statusEventDispatcher);
            _healthManager.MaximumHealth = 5.0f;
        }

        private void OnEnable()
        {
            _healthManager.WireUpEventHandlers();
        }

        private void OnDisable()
        {
            _healthManager.UnhookEventHandlers();
        }
        private void Awake()
        {
            _transform = transform;

            _statusEventDispatcher.Source = _transform;
            _displayController.Transform = _transform;
            _healthManager.Transform = _transform;
        }

        private void Update()
        {
        }

        public void HandleClickedOn()
        {
            _statusEventDispatcher.FireStatusEvent(StatusMessage.CharacterSelected);
        }
    }
}
