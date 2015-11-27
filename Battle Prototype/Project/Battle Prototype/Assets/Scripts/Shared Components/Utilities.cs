using UnityEngine;

namespace Scripts.Shared_Components
{
    public static class Utilities
    {
        public static bool WithinRequiredProximity(Vector3 position, Vector3 targetPosition, Vector2 proximityLimits)
        {
            if ((position.x < targetPosition.x - proximityLimits.x) || (position.x > targetPosition.x + proximityLimits.x)) { return false; }
            if ((position.y < targetPosition.y - proximityLimits.y) || (position.y > targetPosition.y + proximityLimits.y)) { return false; }

            return true;
        }
    }
}
