using UnityEngine;

namespace Scripts.Event_Dispatchers
{
    public class FieldClickEventDispatcher : MonoBehaviour
    {
        public delegate void HandleClick(Vector3 clickPosition);
        public static event HandleClick FieldClickHandler;

        private bool _objectWasSelected;

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _objectWasSelected = false;
            }

            if ((Input.GetMouseButtonUp(0)) && (!_objectWasSelected))
            {
                Debug.Log("Field clicked at " + Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString());

                if (FieldClickHandler != null)
                {
                    FieldClickHandler(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            if (message == StatusMessage.CharacterSelected)
            {
                _objectWasSelected = true;
            }
        }
    }
}