using System;
using UnityEngine;

namespace _IUTHAV.Scripts.CustomUI
{
    public class CustomCursor : MonoBehaviour
    {
       [SerializeField] private CursorType[] _cursorTypes;

        public void SetCursor(CursorState cursorState)
        {
            Texture2D currrentTexture = GetCursorTexture(cursorState);
            Cursor.SetCursor(currrentTexture, new Vector2(128, 128), CursorMode.Auto);
        }

        private Texture2D GetCursorTexture(CursorState cursorState)
        {
            foreach (var cursorType in _cursorTypes)
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