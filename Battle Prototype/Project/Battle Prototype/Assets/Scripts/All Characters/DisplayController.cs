﻿using UnityEngine;

namespace Scripts.All_Characters
{
    public class DisplayController
    {
        private Transform _transform;
        private Animator _animator;

        public Transform Transform { set { _transform = value; _animator = value.FindChild("Character").GetComponent<Animator>(); } }

        public bool IsMoving { set { _animator.SetBool("Moving", value); } }

        public void SetFacing(Vector3 target)
        {
            if ((Mathf.Sign(target.x - _transform.position.x) != Mathf.Sign(_transform.localScale.x)) && (target.x != _transform.position.x))
            {
                _transform.localScale = new Vector3(_transform.localScale.x * -1, _transform.localScale.y, _transform.localScale.z);
            }
        }

        public void TriggerAutoAction()
        {
            _animator.SetBool("AutoAction", true);
        }

        public void CompleteAutoAction()
        {
            _animator.SetBool("AutoAction", false);
        }

        public void TriggerHurtAnimation()
        {
            _animator.SetTrigger("Hurt");
        }

        public void TriggerDeathAnimation()
        {
            _animator.SetTrigger("Death");
        }
    }
}
