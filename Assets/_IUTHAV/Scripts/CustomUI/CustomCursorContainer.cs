using System;
using UnityEngine;

namespace _IUTHAV.Scripts.CustomUI
{
    [Serializable]
    [CreateAssetMenu(fileName = "CustomCursor", menuName = "ScriptableObjects/CustomCursor")]
    public class CustomCursorContainer : ScriptableObject
    {
        [SerializeField] private CursorType[] _cursorTypes;

        public CursorType[] CursorTypes => _cursorTypes;
    }

}