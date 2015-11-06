using UnityEngine;
using Scripts.Generic;

namespace Scripts
{
    public class MouseListener : MonoBehaviour
    {
        public delegate void HandleClick(Vector3 clickPosition, Transform selectedTransform);
        public static event HandleClick ClickHandler;

        private Transform _lastSelected;

        private void OnEnable()
        {
            SelectionController.SelectionStateHandler += HandleSelection;
        }

        private void OnDisable()
        {
            SelectionController.SelectionStateHandler -= HandleSelection;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastSelected = null;
            }

            if (Input.GetMouseButtonUp(0)) 
            {
                if (ClickHandler != null) { ClickHandler(Camera.main.ScreenToWorldPoint(Input.mousePosition), _lastSelected); }
            }
        }

        private void HandleSelection(Transform selected)
        {
            _lastSelected = selected;
        }
    }
}