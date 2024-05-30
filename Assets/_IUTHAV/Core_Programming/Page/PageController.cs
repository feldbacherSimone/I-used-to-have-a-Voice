using System;
using System.Collections;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Page {
    public class PageController : MonoBehaviour {

        public static PageController Instance;

        [SerializeField] private Page[] pages;
        
        [SerializeField] private bool isDebug;
        
        private Hashtable _mPages;
        
#region Unity Functions

        private void Awake() {
            if (!Instance) {
                Configure();
            }
            else {
                Destroy(gameObject);
            }
        }

        private void OnDestroy() {
            _mPages?.Clear();
        }

#endregion

#region Public Functions

        public void TurnPageOn(PageType onPageType) {
            if (onPageType == PageType.None) return;
            if (!PageExists(onPageType)) {
                LogWarning("[" + onPageType + "] does not exist!");
                return;
            }

            Page page = GetPage(onPageType);
            page.gameObject.SetActive(true);
            page.Animate(true);
        }

        public void TurnPageOff(PageType offPageType) {
            if (!PageExists(offPageType)) {
                LogWarning("[" + offPageType + "] does not exist!");
                return;
            }
            Log("Turning [" + offPageType + "] off" );
            Page off = GetPage(offPageType);
            off.Animate(false);
            StopCoroutine("WaitForScreenExit");
            StartCoroutine(WaitForScreenExit(null, off));
        }

        public void SwitchPages(PageType offPageType, PageType onPageType, bool waitForExit = true) {
        
            if (offPageType == PageType.None) {
                LogWarning("Trying to turn off a None page");
            }

            if (!PageExists(offPageType)) {
                LogWarning("[" + offPageType + "] does not exist!");
                return;
            }
            
            Log("Turning [" + offPageType + "] off" );
            Page offPage = GetPage(offPageType);
            if (offPage.gameObject.activeSelf) {
                offPage.Animate(false);
            }
            
            
            if (onPageType != PageType.None) {
                Log("Turning [" + onPageType + "] on");
                Page onPage = GetPage(onPageType);
                if (waitForExit) {
                    StopCoroutine("WaitForScreenExit");
                    StartCoroutine(WaitForScreenExit(onPage, offPage));
                }
                else {
                    TurnPageOn(onPageType);
                }
            }
        }

        public bool PageIsOn(PageType pageType) {
            if (!PageExists(pageType)) {
                Log("Page [" + pageType + "] does not exist");
                return false;
            }
            return GetPage(pageType).IsOn;
        }

        public void AddPage(Page page) {
            if (!PageExists(page.PageType)) {
                LogWarning("Page [" + page.PageType + "] already exists!");
                return;
            }
            RegisterPage(page);
        }

        public void RemovePage(PageType pageType) {
            if (!PageExists(pageType)) {
                LogWarning("Page [" + pageType + "] already exists!");
                return;
            }
            DeRegisterPage(pageType);
        }
        
#endregion

#region Private Functions

        private void Configure() {
             Instance = this;
            _mPages = new Hashtable();
            RegisterAllPages();
            Log("Configured and ready");
            DontDestroyOnLoad(this);
        }
        
        private void RegisterAllPages() {
            foreach (Page page in pages) {
                RegisterPage(page);
            }
        }

        private void RegisterPage(Page page) {
            if (PageExists(page.PageType)) {
                LogWarning("Cannot register [" + page.gameObject.name +
                           "], it already is registered: Check property 'type' on your screen-gameobject");
                return;
            }
            
            _mPages.Add(page.PageType, page);
            Log("Registered new Page [" + page.PageType + "]");
        }

        private void DeRegisterPage(PageType pageType) {
            if (!PageExists(pageType)) {
                LogWarning("Cannot delete [" + pageType +
                           "], it´s not registered");
                return;
            }
            _mPages.Remove(pageType);
            Log("Deleted screen [" + pageType + "]");
        }

        private Page GetPage(PageType pageType) {
            if (!PageExists(pageType)) {
                LogWarning("Page [" + pageType + "] does not exist!");
                return null;
            }
            return (Page)_mPages[pageType];
        }
        private bool PageExists(PageType pageType) {
            
            return _mPages.Contains(pageType);
        }
        
        private IEnumerator WaitForScreenExit(Page on, Page off) {
        
            while (off.TargetState != Page.FLAG_NONE) {
                yield return null;
            }
            off.gameObject.SetActive(false);
            if (on != null) TurnPageOn(on.PageType);
        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[PageController] " + msg);
        }
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[PageController] " + msg);
        }
        
#endregion


    }
}
