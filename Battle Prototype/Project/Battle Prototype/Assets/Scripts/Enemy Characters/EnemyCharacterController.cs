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

        public EnemyCharacterController()
            : base()
        {
            _statusEventDispatcher = new StatusEventDispatcher();
            _displayController = new DisplayController();
        }

        private void OnEnable()
        {
            // TODO: Wire up event handlers
        }

        private void OnDisable()
        {
            // TODO: Unhook event handlers
        }
        private void Awake()
        {
            _transform = transform;

            _statusEventDispatcher.Source = _transform;
            _displayController.Transform = _transform;
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
