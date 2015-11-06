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
        private AutoActionController _autoActionController;

        // This will come out when we get target selection working
        public GameObject Target;

        public EnemyCharacterController()
            : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _displayController = new DisplayController();
            _motionEngine = new MotionEngine();
            _healthManager = new HealthManager(_statusEventDispatcher, _displayController);
            _autoActionController = new AutoActionController(_motionEngine, _displayController, _statusEventDispatcher);
        }

        private void OnEnable()
        {
            _autoActionController.WireUpEventHandlers();
            _healthManager.WireUpEventHandlers();
        }

        private void OnDisable()
        {
            _autoActionController.UnhookEventHandlers();
            _healthManager.UnhookEventHandlers();
        }

        private void Awake()
        {
            _transform = transform;

            ConnectComponentsToCharacter();
            SetCharacterStatistics();

            // TODO: Target selection process
            _autoActionController._actionTarget = Target.transform;
        }

        private void ConnectComponentsToCharacter()
        {
            _statusEventDispatcher.Source = _transform;
            _motionEngine.Transform = _transform;
            _displayController.Transform = _transform;
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
            _autoActionController.Update();
        }

        public void HandleClickedOn()
        {
            _statusEventDispatcher.FireStatusEvent(StatusMessage.CharacterSelected);
        }
    }
}
