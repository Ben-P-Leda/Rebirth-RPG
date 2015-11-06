using UnityEngine;
using Scripts.Event_Dispatchers;

namespace Scripts.Data_Models
{
    public class CharacterData
    {
        public float ActionLocationOffset { get; set; }
        public Vector2 RequiredTargetProximity { get; set; }
        public float AutoActionCooldown { get; set; }
        public StatusMessage AutoActionEffect { get; set; }
        public float AutoActionEffectMagnitude { get; set; }
        public float Health { get; set; }
        public float MovementSpeed { get; set; }
    }
}
