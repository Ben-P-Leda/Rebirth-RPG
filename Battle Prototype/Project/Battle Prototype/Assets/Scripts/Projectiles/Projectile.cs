using UnityEngine;
using Scripts.Event_Dispatchers;
using Scripts.Shared_Components;

namespace Scripts.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public GameObject Launcher;
        public Vector2 LaunchOffset;

        private Transform _tranform;
        private Transform _launcherTransform;
        private Transform _targetTransform;
        private SpriteRenderer _spriteRenderer;

        private MotionEngine _motionEngine;

        public Projectile() : base()
        {
            _targetTransform = null;

            _motionEngine = new MotionEngine();
            _motionEngine.MovementSpeed = Motion_Speed;
        }

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            if ((originator == _launcherTransform) && (message == StatusMessage.RangeEffectAttack))
            {
                _targetTransform = target;

                _tranform.position = _launcherTransform.position 
                    + new Vector3(LaunchOffset.x * Mathf.Sign(_launcherTransform.localScale.x), LaunchOffset.y, 0.0f);

                _tranform.localScale = new Vector3(Mathf.Abs(_tranform.localScale.x) * Mathf.Sign(_launcherTransform.localScale.x),
                    _tranform.localScale.y, _tranform.localScale.z);

                _spriteRenderer.enabled = true;
            }
        }

        private void Awake()
        {
            _tranform = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _motionEngine.Transform = transform;

            _launcherTransform = Launcher.transform;
        }

        private void FixedUpdate()
        {
            if (_targetTransform != null)
            {
                _motionEngine.MoveTowardsPosition(_targetTransform.position + new Vector3(0.0f, Vertical_Offset, 0.0f));
            }
        }

        // TODO: Should be from some kind of config...
        private const float Motion_Speed = 10.0f;
        private const float Vertical_Offset = 0.3f;
    }
}
