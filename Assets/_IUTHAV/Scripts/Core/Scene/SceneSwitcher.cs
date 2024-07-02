using System.Collections;
using _IUTHAV.Scripts.Core.Page;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _IUTHAV.Scripts.Core.Scene {
    public class SceneSwitcher : MonoBehaviour {

        public void LoadMainMenu () {
            //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            StartCoroutine(WaitAndLoadByString("MainMenu"));
        }

        public void LoadByString(string sceneName) {
            //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            StartCoroutine(WaitAndLoadByString(sceneName));
        }
        
        private IEnumerator WaitAndLoadByString(string sceneName) {

            PageController controller = GameObject.FindWithTag("SingletonContainer")?.GetComponent<PageController>();
            
            if (controller != null) {
                controller.TurnPageOn(PageType.LoadingPage);
            }

            yield return new WaitForSeconds(2.0f);
            
            SceneLoader.LoadSingle(sceneName, PageType.LoadingPage);
            
            if (controller != null) {
                controller.TurnPageOff(PageType.LoadingPage);
            }
            
            yield return null;
        }
        
    }
}
