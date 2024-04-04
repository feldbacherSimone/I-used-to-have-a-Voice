using _IUTHAV.Core_Programming.Page;
using UnityEngine;

namespace _IUTHAV.Core_Programming.Scene {
    public class SceneSwitcher : MonoBehaviour {

        public void LoadMainMenu () {
            
            SceneLoader.LoadSingle(SceneType.MainMenu.ToString(), PageType.LoadingPage);
        }

        public void LoadByString(string sceneName) {
            SceneLoader.LoadSingle(sceneName, PageType.LoadingPage, 2000);
        }
        
    }
}
