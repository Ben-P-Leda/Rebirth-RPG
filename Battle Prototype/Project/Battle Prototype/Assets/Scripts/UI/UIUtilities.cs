using UnityEngine;

namespace Scripts.UI
{
    public static class UIUtilities
    {
        private static float _scaling = 0.0f;
        public static float Scaling
        {
            get
            {
                if (_scaling == 0.0f)
                {
                    _scaling = Screen.height / One_To_One_Screen_Height;
                }
                return _scaling;
            }
        }

        private const float One_To_One_Screen_Height = 675.0f;
    }
}