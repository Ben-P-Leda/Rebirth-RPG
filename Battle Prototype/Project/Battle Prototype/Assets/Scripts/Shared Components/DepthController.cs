using UnityEngine;
using System.Collections.Generic;

namespace Scripts.Shared_Components
{
    public class DepthController : MonoBehaviour
    {
        private Transform _transform;
        private Dictionary<SpriteRenderer, int> _depthOffsets;

        public int BaseOffset;

        private void Awake()
        {
            _transform = transform;
            _depthOffsets = new Dictionary<SpriteRenderer, int>();

            AddDepthOffsets(_transform);
        }

        private void AddDepthOffsets(Transform current)
        {
            SpriteRenderer renderer = current.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                _depthOffsets.Add(renderer, renderer.sortingOrder);
            }

            for (int i = 0; i < current.childCount; i++)
            {
                AddDepthOffsets(current.GetChild(i));
            }
        }

        private void Update()
        {
            int depthBase = Mathf.FloorToInt(-_transform.position.y * 1000.0f);
            foreach (KeyValuePair<SpriteRenderer, int> kvp in _depthOffsets)
            {
                kvp.Key.sortingOrder = depthBase + BaseOffset + kvp.Value;
            }
        }
    }
}