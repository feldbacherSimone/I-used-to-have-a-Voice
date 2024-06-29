using System;
using UnityEngine;

namespace _IUTHAV.Scripts.CustomUI
{
    public static class CustomCursor
    {
        private static CustomCursorContainer _customCursorContainer; 

        public static void SetCursor(CursorState cursorState)
        {
            if (!_customCursorContainer)
            {
                _customCursorContainer = Resources.Load<CustomCursorContainer>("ScriptableObjects/CustomCursor");
            }
            Texture2D currrentTexture = GetCursorTexture(cursorState);
            Cursor.SetCursor(currrentTexture, new Vector2(64, 64), CursorMode.Auto);
        }

        private static Texture2D GetCursorTexture(CursorState cursorState)
        {
            foreach (var cursorType in _customCursorContainer.CursorTypes)
            {
                if (cursorType.CursorState == cursorState)
                {
                    return cursorType.CursorTexture2D;
                }
            }

            return null;
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "CustomCursor", menuName = "ScriptableObjects/CustomCursor")]
    public class CustomCursorContainer : ScriptableObject
    {
        [SerializeField] private CursorType[] _cursorTypes;

        public CursorType[] CursorTypes => _cursorTypes;
    }

    [Serializable]
    public class CursorType
    {
        [SerializeField] private Texture2D cursorTexture2D;
        [SerializeField] private CursorState _cursorState;

        public Texture2D CursorTexture2D => cursorTexture2D;

        public CursorState CursorState => _cursorState;
    }

    public enum CursorState
    {
        Default,
        Interact,
        Talk, 
    }
}