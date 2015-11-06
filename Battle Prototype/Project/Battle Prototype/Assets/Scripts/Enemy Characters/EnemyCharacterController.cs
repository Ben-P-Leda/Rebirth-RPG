using UnityEngine;
using Scripts;
using Scripts.Data_Models;
using Scripts.Event_Dispatchers;
using Scripts.All_Characters;

namespace Scripts.Enemy_Characters
{
    public class EnemyCharacterController : MonoBehaviour
    {
        private Transform _transform;

        private StatusEventDispatcher _statusEventDispatcher;
        private MotionEngine _motionEngine;
        private DisplayController _displayController;
        private HealthManager _healthManager;

        public EnemyCharacterController()
            : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _displayController = new DisplayController();
            _motionEngine = new MotionEngine();
            _healthManager = new HealthManager(_statusEventDispatcher, _displayController);
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

            ConnectComponentsToCharacter();
            SetCharacterStatistics();
        }

        private void ConnectComponentsToCharacter()
        {
            _statusEventDispatcher.Source = _transform;
            _motionEngine.Transform = _transform;
            _displayController.Transform = _transform;
            _healthManager.Transform = _transform;
        }

        private void SetCharacterStatistics()
        {
            CharacterData stats = CharacterDataProvider.Instance.GetCharacterData(_transform);

            _motionEngine.MovementSpeed = stats.MovementSpeed;

            _healthManager.MaximumHealth = stats.Health;

            //_autoActionController.ActionLocationOffset = stats.ActionLocationOffset;
            //_autoActionController.RequiredTargetProximity = stats.RequiredTargetProximity;
            //_autoActionController.Cooldown = stats.AutoActionCooldown;
            //_autoActionController.ActionInvokationStatusEvent = stats.AutoActionEffect;
            //_autoActionController.ActionEffectValue = stats.AutoActionEffectMagnitude;
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
