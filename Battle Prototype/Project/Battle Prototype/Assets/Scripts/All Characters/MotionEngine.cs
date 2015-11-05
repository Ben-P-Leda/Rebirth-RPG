using UnityEngine;

namespace Scripts.All_Characters
{
    public class MotionEngine
    {
        private Transform _transform;
        private Rigidbody2D _rigidbody2D;

        public Transform Transform { set { _transform = value; _rigidbody2D = value.GetComponent<Rigidbody2D>(); } }
        public float MovementSpeed { private get; set; }

        public void MoveTowardsPosition(Vector3 targetPosition)
        {
            Vector2 vectorToTarget = targetPosition - _transform.position;
            _rigidbody2D.velocity = vectorToTarget.normalized * MovementSpeed;
        }

        public void StopMoving()
        {
            _rigidbody2D.velocity = Vector3.zero;
        }
    }
}
