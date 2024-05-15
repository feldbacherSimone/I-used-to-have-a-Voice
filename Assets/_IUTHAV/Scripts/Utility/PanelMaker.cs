using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Cinemachine;
using Unity.VisualScripting;
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

        [SerializeField] private int cameraOutputMask = 0;

        [SerializeField] private string shotName;

        [SerializeField] private bool createRenderTextureAndCamera = false;
        [FormerlySerializedAs("renderTextureTargetId")]
        [Tooltip("Decide which render texture should be displayed")]
        [SerializeField] private string renderTextureTargetName;
        
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
                _mRenderTexturePath = "RenderTextures/" + _mSceneName + "/";
                
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
                "Assets/_IUTHAV/Resources/" + _mRenderTexturePath + TextureName + "_" + shotName + ".renderTexture");
            
        }

        private void CreateCamPackage() {

            string camName = CamPackageName + "_" + shotName;
        
            if (GameObject.Find(camName) != null) {
                
                LogWarning(camName + " already exists! Check your Hierarchy or try overriding it's id!");
                return;
            }
        
            GameObject camObj = (GameObject)PrefabUtility.InstantiatePrefab(camPackagePrefab);
            
            camObj.name = camName;

            camObj.transform.parent = GameObject.FindWithTag("CamPackageContainer").transform;

            UnityEngine.Camera camera = camObj.GetComponentInChildren<UnityEngine.Camera>();
            camera.gameObject.name = "C" + "_" + shotName + "_" + cameraOutputMask;
            
            RenderTexture texture = Resources.Load<RenderTexture>(_mRenderTexturePath + TextureName + "_" + shotName);

            if (texture == null) {
                LogWarning("No texture of name could be found!");
                return;
            }
            
            camera.targetTexture = texture;

            CinemachineBrain cBrain = camObj.GetComponentInChildren<CinemachineBrain>();
            OutputChannels e = OutputChannels.Channel01;
            cBrain.ChannelMask = (OutputChannels)Enum.GetValues(e.GetType()).GetValue(cameraOutputMask+1);
            
            LogWarning("Successfully created Camera and RenderTexture with id [" + cameraOutputMask + "]");
        }
        
        

        private void CreatePanel() {
        
            string pName = PanelName + "_" + shotName;
        
            if (GameObject.Find(pName) != null) {
                
                LogWarning(pName + " already exists! Check your Hierarchy or try overriding it's id!");
                return;
            }

            if (renderTextureTargetName == "") renderTextureTargetName = shotName;
            RenderTexture texture = Resources.Load<RenderTexture>(_mRenderTexturePath + TextureName + "_" + renderTextureTargetName);

            if (texture == null) {
                LogWarning("No RenderTexture with id [" + cameraOutputMask + "] found! Check Resources/Panels!");
                return;
            }

            GameObject panelObj = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab);

            panelObj.name = pName;

            panelObj.transform.SetParent(GameObject.FindWithTag("PanelContainer").transform, false);
            
            RawImage rImg = panelObj.GetComponent<RawImage>();
            rImg.texture = texture;
            
            rImg.SetNativeSize();
            
            if (renderTextureTargetName == "") renderTextureTargetName = shotName;
            GameObject camPackage = GameObject.Find("CamPackage" + "_" + renderTextureTargetName);
            
            ComicPanel.Panel panelC = panelObj.GetOrAddComponent<ComicPanel.Panel>();
                panelC.SetReferences(camPackage.transform.GetChild(0).gameObject,
                camPackage.transform.GetChild(2).gameObject.transform,
                camPackage.GetComponentInChildren<UnityEngine.Camera>()
            );
            
            LogWarning("Successfully created Panel [" + shotName + "]");

        }

        private void DeleteCamPackage(string camSuffix) {

            var obj = GameObject.FindWithTag("CamPackageContainer");

            try {
                GameObject destroyObject = null;
                for (int i = 0; i < obj.transform.childCount; i++) {

                    if (obj.transform.GetChild(i).gameObject.name.EndsWith(camSuffix)) {

                        destroyObject = obj.transform.GetChild(i).gameObject;

                    }

                }
                
                if (destroyObject != null) DestroyImmediate(destroyObject);
            }
            catch {
                LogWarning("Error in deleting CamPackage with suffix " + camSuffix);
            }

            

        }

        private void DeletePanel(string panelSuffix) {

            var obj = GameObject.Find(PanelName + "_" + panelSuffix);

            if (obj.TryGetComponent(out ComicPanel.Panel panel)) {
                
                DestroyImmediate(obj);
                
            }

        }

        private void LogWarning(string msg) {
            
            Debug.LogWarning("[CamPrefabMaker] " + msg);
        }

    }
}
