using UnityEngine;
using Scripts.Generic;

namespace Scripts.Player_Characters
{
    public class ActionController : MonoBehaviour
    {
        private CharacterActionController _characterActionController;

        private Transform _transform;

        private bool _selected;

        public ActionController() : base()
        {
            _characterActionController = new CharacterActionController();
        }

        private void OnEnable()
        {
            _characterActionController.WireUpEventHandlers();
            MouseListener.ClickHandler += HandleClick;
        }

        private void OnDisable()
        {
            _characterActionController.UnhookEventHandlers();
            MouseListener.ClickHandler -= HandleClick;
        }

        private void Awake()
        {
            _transform = transform;

            _characterActionController.Transform = _transform;
            _characterActionController.RigidBody = GetComponent<Rigidbody2D>();
            _characterActionController.Animator = _transform.FindChild("Character").GetComponent<Animator>();

            _characterActionController.MovementTarget = _transform.position;
        }

        private void HandleClick(Vector3 clickPosition, Transform selectedTransform)
        {
            if (selectedTransform != null) { RespondToCharacterClick(selectedTransform); }
            else { RespondToGenericClick(clickPosition); }
        }

        private void RespondToCharacterClick(Transform selection)
        {
            if ((_selected) && (selection.tag == "Enemy")) 
            {
                _characterActionController.ActionTarget = selection; 
            }

            _selected = (_transform == selection);
        }

        private void RespondToGenericClick(Vector3 clickPosition)
        {
            if (_selected)
            {
                _characterActionController.MovementTarget = new Vector3(clickPosition.x, clickPosition.y, _transform.position.z);
                _characterActionController.ActionTarget = null;
            }
        }

        private void Update()
        {
            _characterActionController.Update();
        }
    }
}