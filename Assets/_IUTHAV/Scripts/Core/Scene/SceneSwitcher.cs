using System.Collections;
using _IUTHAV.Scripts.Core.Page;
using UnityEngine;

namespace _IUTHAV.Scripts.Core.Scene {
    public class SceneSwitcher : MonoBehaviour {

        public void LoadMainMenu () {
            
            SceneLoader.LoadSingle(SceneType.MainMenu.ToString(), PageType.LoadingPage);
        }

        public void LoadByString(string sceneName) {
            StartCoroutine(WaitAndLoadByString(sceneName));
        }
        
        private IEnumerator WaitAndLoadByString(string sceneName) {
            
            if (PageController.Instance != null) {
                PageController.Instance.TurnPageOn(PageType.LoadingPage);
            }

            yield return new WaitForSeconds(2.0f);
            
            SceneLoader.LoadSingle(sceneName, PageType.LoadingPage);
            
            if (PageController.Instance != null) {
                PageController.Instance.TurnPageOff(PageType.LoadingPage);
            }
            
            yield return null;
        }
        
    }
}
