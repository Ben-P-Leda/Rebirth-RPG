using UnityEngine;
using Scripts.Generic;

namespace Scripts.Player_Characters
{
    public class ActionController : MonoBehaviour
    {
        private CharacterActionController _characterActionController;
        private ActionTargetResolver _actionTargetResolver;

        private Transform _transform;

        private string _faction;
        private bool _selected;
        private bool _currentSelectionTargetsAllies;

        public ActionController() : base()
        {
            _characterActionController = new CharacterActionController();
            _actionTargetResolver = new ActionTargetResolver();

            _currentSelectionTargetsAllies = false;
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
            _faction = _transform.tag.Substring(0, _transform.tag.IndexOf("-"));

            _characterActionController.Transform = _transform;
            _characterActionController.RigidBody = GetComponent<Rigidbody2D>();
            _characterActionController.Animator = _transform.FindChild("Character").GetComponent<Animator>();

            _characterActionController.MovementTarget = _transform.position;

            _actionTargetResolver.InitializeTargetType(_transform);
        }

        private void HandleClick(Vector3 clickPosition, Transform selectedTransform)
        {
            if (selectedTransform != null) { RespondToCharacterClick(selectedTransform); }
            else { RespondToGenericClick(clickPosition); }
        }

        private void RespondToCharacterClick(Transform selection)
        {
            if ((_selected) && (_actionTargetResolver.IsValidTarget(selection))) 
            {
                _characterActionController.ActionTarget = selection;
                Debug.Log(_transform.tag + ": " + selection.tag + " is a valid target");
            }

            if (_currentSelectionTargetsAllies)
            {
                _selected = false;
                _currentSelectionTargetsAllies = false;

                Debug.Log(_transform.tag + " deselected as last selected targets allies");
            }
            else
            {
                _selected = (_transform == selection);
                _currentSelectionTargetsAllies = (_transform.tag == _faction + "-Healer");

                Debug.Log(_transform.tag + " selection state is " + _selected.ToString());
            }
        }

        private void RespondToGenericClick(Vector3 clickPosition)
        {
            if (_selected)
            {
                Debug.Log(_transform.tag + " responding to positioning click");

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