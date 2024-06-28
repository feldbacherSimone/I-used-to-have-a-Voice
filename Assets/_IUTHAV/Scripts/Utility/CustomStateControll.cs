using System;
using _IUTHAV.Scripts.CustomUI;
using UnityEngine;

namespace _IUTHAV.Scripts.Utility
{
    public class CustomStateControll : MonoBehaviour
    {
        [SerializeField] private ScrollBackGround _scrollBackGround;

        private int bookmarkCount; 
        private void Start()
        {
            bookmarkCount = _scrollBackGround.bookmarkCount;
        }

        public void ClearAllBookmarks()
        {
            for (int i = 0; i < bookmarkCount; i++)
            {
                _scrollBackGround.NextBookmark();
            }
        }

        public void NextBookmark()
        {
            if (bookmarkCount > 0)
            {
                _scrollBackGround.NextBookmark();
            }
        }
    }
}