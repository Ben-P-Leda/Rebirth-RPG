using UnityEngine;
using System;
using Scripts;
using Scripts.Data_Models;
using Scripts.Event_Dispatchers;

namespace Scripts
{
    public class CharacterDataProvider
    {
        private static CharacterDataProvider _instance = null;
        public static CharacterDataProvider Instance
        {
            get
            {
                if (_instance == null) { _instance = new CharacterDataProvider(); }
                return _instance;
            }
        }

        public CharacterDataProvider()
        {
            if (_instance == null) 
            {
                _instance = this;
            }
            else
            {
                throw new Exception("Cannot have more than one Character Data Provider!");
            }
        }

        public CharacterData GetCharacterData(Transform characterTransform)
        {
            CharacterData data = new CharacterData();

            SetFactionBasedValues(CharacterUtilities.GetFaction(characterTransform), data);
            SetRoleBasedValues(CharacterUtilities.GetRole(characterTransform), data);

            return data;
        }

        private void SetFactionBasedValues(string faction, CharacterData data)
        {
            if (faction == "player")
            {
                data.Health = 1.0f;
                data.MovementSpeed = 1.0f;
                data.AutoActionCooldown = 1.75f;
            }
            else
            {
                data.MovementSpeed = 0.75f;
                data.AutoActionCooldown = 2.0f;
            }
        }

        private void SetRoleBasedValues(string role, CharacterData data)
        {
            if (role == "fighter")
            {
                data.Health += 10.0f;
                data.ActionLocationOffset = 0.75f;
                data.RequiredTargetProximity = new Vector2(0.05f, 0.05f);
                data.AutoActionEffect = StatusMessage.ReduceHealth;
                data.AutoActionEffectMagnitude = 1.0f;
                data.MovementSpeed -= 0.1f;
            }
            else if (role == "healer")
            {
                data.Health += 5.0f;
                data.RequiredTargetProximity = new Vector2(3.0f, 3.0f);
                data.AutoActionEffect = StatusMessage.ReduceHealth;
                data.AutoActionEffectMagnitude = 1.0f;
                data.MovementSpeed += 0.1f;
            }
        }


    }
}
