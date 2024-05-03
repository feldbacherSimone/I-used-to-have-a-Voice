using System;
using _IUTHAV.Scripts.Panel;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _IUTHAV.Scripts.Utility {

    [CreateAssetMenu(fileName = "CamGeneratorObject", menuName = "ScriptableObjects/CamGeneratorObject", order = 3)]
    public class PanelMaker : ScriptableObject {

        [SerializeField] private GameObject camPackagePrefab;
        [SerializeField] private RenderTexture renderTextureTemplate;
        [SerializeField] private GameObject panelPrefab;
        [SerializeField] [Range(10, 4000)] private int renderTextureWidth;
        [SerializeField] [Range(10, 4000)] private int renderTextureHeight;

        [SerializeField] private int currentId = 0;

        [SerializeField] private bool createRenderTextureAndCamera = false;

        [SerializeField] private string panelName;
        [Tooltip("Decide which render texture should be displayed")]
        [SerializeField] private int renderTextureTargetId;
        [SerializeField] private bool createPanel = false;

        private const string CamPackageName = "CamPackage";
        private const string TextureName = "RenderTexture";
        private const string PanelName = "Panel";

        private string _mSceneName;

        private string _mRenderTexturePath;

        private void OnValidate() {

            if (createRenderTextureAndCamera || createPanel) {
                _mSceneName = SceneManager.GetActiveScene().name;
                
                //Set the path:
                _mRenderTexturePath = "RenderTextures/" + _mSceneName + "_" + TextureName + "_";
                
            }

            if (createRenderTextureAndCamera) {
                createRenderTextureAndCamera = false;
                
                CreateRenderTexture();
                CreateCamPackage();
            }

            if (createPanel) {
                createPanel = false;
                
                CreatePanel();
            }
        }
        
        private void CreateRenderTexture() {

            RenderTexture texture = new RenderTexture(
                renderTextureWidth, 
                renderTextureHeight, 
                renderTextureTemplate.graphicsFormat,
                renderTextureTemplate.depthStencilFormat
            );

            AssetDatabase.CreateAsset(texture,
                "Assets/_IUTHAV/Resources/" + _mRenderTexturePath + currentId + ".renderTexture");
            
        }

        private void CreateCamPackage() {

            string camName = CamPackageName + "_" + currentId;
        
            if (GameObject.Find(camName) != null) {
                
                LogWarning(camName + " already exists! Check your Hierarchy or try overriding it's id!");
                return;
            }
        
            GameObject camObj = (GameObject)PrefabUtility.InstantiatePrefab(camPackagePrefab);
            
            camObj.name = camName;

            camObj.transform.parent = GameObject.FindWithTag("CamPackageContainer").transform;

            UnityEngine.Camera camera = camObj.GetComponentInChildren<UnityEngine.Camera>();
            camera.gameObject.name = "C" + "_" + currentId;
            
            RenderTexture texture = Resources.Load<RenderTexture>(_mRenderTexturePath + currentId);
            
            camera.targetTexture = texture;

            CinemachineBrain cBrain = camObj.GetComponentInChildren<CinemachineBrain>();
            OutputChannels e = OutputChannels.Channel01;
            cBrain.ChannelMask = (OutputChannels)Enum.GetValues(e.GetType()).GetValue(currentId+1);
            
            LogWarning("Successfully created Camera and RenderTexture with id [" + currentId + "]");
        }
        
        

        private void CreatePanel() {
        
            string pName = PanelName + "_" + panelName;
        
            if (GameObject.Find(pName) != null) {
                
                LogWarning(pName + " already exists! Check your Hierarchy or try overriding it's id!");
                return;
            }
            
            RenderTexture texture = Resources.Load<RenderTexture>(_mRenderTexturePath + renderTextureTargetId);

            if (texture == null) {
                LogWarning("No RenderTexture with id [" + currentId + "] found! Check Resources/Panels!");
                return;
            }

            GameObject panelObj = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab);

            panelObj.name = pName;

            panelObj.transform.SetParent(GameObject.FindWithTag("PanelContainer").transform, false);
            
            RawImage rImg = panelObj.GetComponent<RawImage>();
            rImg.texture = texture;
            
            rImg.SetNativeSize();

            GameObject camPackage = GameObject.Find("CamPackage_"+renderTextureTargetId);
            Panel.Panel panelC = panelObj.GetComponent<Panel.Panel>();
                panelC.SetReferences(
                camPackage.transform.GetChild(0).gameObject,
                camPackage.transform.GetChild(2).gameObject.transform,
                camPackage.GetComponentInChildren<UnityEngine.Camera>()
            );
            
            LogWarning("Successfully created Panel [" + panelName + "]");

        }

        private void LogWarning(string msg) {
            
            Debug.LogWarning("[CamPrefabMaker] " + msg);
        }

    }
}
