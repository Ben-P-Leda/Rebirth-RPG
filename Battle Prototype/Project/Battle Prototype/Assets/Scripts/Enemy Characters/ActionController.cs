using UnityEngine;
using Scripts.Generic;

namespace Scripts.Enemy_Characters
{
    public class ActionController : MonoBehaviour
    {
        private CharacterActionController _characterActionController;

        public GameObject Target;

        public ActionController() : base()
        {
            _characterActionController = new CharacterActionController();
        }

        private void OnEnable()
        {
            _characterActionController.WireUpEventHandlers();
        }

        private void OnDisable()
        {
            _characterActionController.UnhookEventHandlers();
        }

        private void Awake()
        {
            _characterActionController.Transform = transform;
            _characterActionController.RigidBody = GetComponent<Rigidbody2D>();
            _characterActionController.Animator = transform.FindChild("Character").GetComponent<Animator>();
            _characterActionController.ActionTarget = Target.transform;
        }

        private void Update()
        {
            _characterActionController.Update();
        }
    }
}