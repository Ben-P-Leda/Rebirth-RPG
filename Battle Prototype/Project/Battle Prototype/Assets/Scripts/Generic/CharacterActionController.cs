using UnityEngine;

namespace Scripts.Generic
{
    public class CharacterActionController
    {
        private Transform _transform;
        private Rigidbody2D _rigidBody;
        private Animator _characterAnimator;
        private StatusEventDispatcher _statusEventDispatcher;

        private bool _actionInProgress;
        private float _autoActionCooldown;

        public Transform Transform { set { _transform = value; _statusEventDispatcher.Source = value; } }
        public Rigidbody2D RigidBody { set { _rigidBody = value; } }
        public Animator Animator { set { _characterAnimator = value; } }

        public Transform ActionTarget { get; set; }
        public Vector3 MovementTarget { get; set; }

        public CharacterActionController()
        {
            ActionTarget = null;
            MovementTarget = Vector3.zero;

            _actionInProgress = false;
            _autoActionCooldown = 0.0f;

            _statusEventDispatcher = new StatusEventDispatcher();
        }

        public void WireUpEventHandlers()
        {
            AnimationEventDispatcher.AnimationEventHandler += HandleAnimationEvent;
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        public void UnhookEventHandlers()
        {
            AnimationEventDispatcher.AnimationEventHandler -= HandleAnimationEvent;
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        public void Update()
        {
            if (!_actionInProgress)
            {
                if (ActionTarget != null)
                {
                    SetMovementTargetToActionTargetPosition();
                    FaceTarget(ActionTarget.position);
                }
                else
                {
                    FaceTarget(MovementTarget);
                }
            }

            if (!CloseEnoughToMovementTarget())
            {
                if (!_actionInProgress)
                {
                    MoveTowardsTarget();
                }
            }
            else
            {
                StopMoving();
                if ((ActionTarget != null) && (_autoActionCooldown <= 0.0f))
                {
                    StartAutoAction();
                }
            }

            _autoActionCooldown = Mathf.Max(_autoActionCooldown - Time.deltaTime, 0.0f);
        }

        private void SetMovementTargetToActionTargetPosition()
        {
            float x = (_transform.position.x > ActionTarget.position.x)
                ? ActionTarget.position.x + Movement_Offset_From_Action_Target
                : ActionTarget.position.x - Movement_Offset_From_Action_Target;

            MovementTarget = new Vector3(x, ActionTarget.position.y, 0.0f);
        }

        private bool WithinRange(float position, float target, float range)
        {
            return ((position >= target - range) && (position <= target + range));
        }

        private bool CloseEnoughToMovementTarget()
        {
            return (WithinRange(_transform.position.x, MovementTarget.x, AutoAction_Horizontal_Range)
                && WithinRange(_transform.position.y, MovementTarget.y, AutoAction_Vertical_Range));
        }

        private void FaceTarget(Vector3 targetPosition)
        {
            if (Mathf.Sign(targetPosition.x - _transform.position.x) != Mathf.Sign(_transform.localScale.x))
            {
                _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z);
            }
        }

        private void MoveTowardsTarget()
        {
            Vector2 direction = MovementTarget - _transform.position;
            _rigidBody.velocity = direction.normalized * Speed;

            _characterAnimator.SetBool("Moving", true);
            _characterAnimator.speed = Speed;
        }

        private void StopMoving()
        {
            _characterAnimator.SetBool("Moving", false);
            _rigidBody.velocity = Vector3.zero;
        }

        private void StartAutoAction()
        {
            _actionInProgress = true;
            _characterAnimator.SetBool("AutoAction", true);
        }

        private void HandleAnimationEvent(Transform source, AnimationEventDispatcher.AnimationEvent message)
        {
            if (source == _transform)
            {
                switch (message)
                {
                    case AnimationEventDispatcher.AnimationEvent.AutoActionEffectOccurs: InvokeAutoActionEffect(); break;
                    case AnimationEventDispatcher.AnimationEvent.AutoActionComplete: CompleteAutoAction(); break;
                }
            }
        }

        private void InvokeAutoActionEffect()
        {
            _statusEventDispatcher.FireStatusEvent(ActionTarget, "auto action", 1.0f);
        }

        private void CompleteAutoAction()
        {
            _actionInProgress = false;
            _characterAnimator.SetBool("AutoAction", false);

            _autoActionCooldown = Time_Between_Auto_Actions;
        }

        private void HandleStatusEvent(Transform source, Transform target, string message, float value)
        {
            if (target == _transform)
            {
                Debug.Log(string.Format("{3} - SE from {0}: {1} (value {2})", source.name, message, value, target.name));
            }
        }

        private const float Movement_Offset_From_Action_Target = 0.75f;
        private const float AutoAction_Horizontal_Range = 0.05f;
        private const float AutoAction_Vertical_Range = 0.05f;

        private const float Speed = 0.75f;

        private const float Time_Between_Auto_Actions = 2.0f;

    }
}
