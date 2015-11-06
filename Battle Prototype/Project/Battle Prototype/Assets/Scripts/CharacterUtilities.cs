using UnityEngine;

namespace Scripts
{
    public static class CharacterUtilities
    {
        public static string GetFaction(Transform characterTransform)
        {
            if (characterTransform.tag.Contains("-"))
            {
                return characterTransform.tag.Split('-')[0].ToLower();
            }
            else
            {
                return "";
            }
        }

        public static string GetRole(Transform characterTransform)
        {
            if (characterTransform.tag.Contains("-"))
            {
                return characterTransform.tag.Split('-')[1].ToLower();
            }
            else
            {
                return "";
            }
        }

        public static bool CharacterTargetsAllies(Transform characterTransform)
        {
            if (characterTransform == null) { return false; }

            string role = GetRole(characterTransform);
            return Roles_Targeting_Allies.Contains(role);
        }

        public static bool CharactersAreAllies(Transform characterA, Transform characterB)
        {
            if ((characterA == null) || (characterB == null)) { return false; }

            return (GetFaction(characterA) == GetFaction(characterB));
        }

        private const string Roles_Targeting_Allies = "healer";
    }
}
