using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// https://www.febucci.com/2022/05/custom-post-processing-in-urp/ dankeee tutorial
// https://www.youtube.com/watch?v=9fa4uFm1eCE auch danke, cooles video 
// https://www.youtube.com/watch?v=LMqio9NsqmM coolstes video
namespace _IUTHAV.Scripts.TechArt
{
    public class OutlineRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        private class ScreenSpaceOutlineSettings {

            [Header("General Outline Settings")]
            public Color outlineColor = Color.black;
            [Range(0.0f, 20.0f)]
            public float outlineScale = 1.0f;
        
            [Header("Depth Settings")]
            [Range(0.0f, 100.0f)]
            public float depthThreshold = 1.5f;
            [Range(0.0f, 500.0f)]
            public float robertsCrossMultiplier = 100.0f;

            [Header("Normal Settings")]
            [Range(0.0f, 1.0f)]
            public float normalThreshold = 0.4f;

            [Header("Depth Normal Relation Settings")]
            [Range(0.0f, 2.0f)]
            public float steepAngleThreshold = 0.2f;
            [Range(0.0f, 500.0f)]
            public float steepAngleMultiplier = 25.0f;

        }
        
        [System.Serializable]
        private class ViewSpaceNormalsTextureSettings {

            [Header("General Scene View Space Normal Texture Settings")]
            public RenderTextureFormat colorFormat;
            public int depthBufferBits = 16;
            public FilterMode filterMode;
            public Color backgroundColor = Color.black;

            [Header("View Space Normal Texture Object Draw Settings")]
            public PerObjectData perObjectData;
            public bool enableDynamicBatching;
            public bool enableInstancing;

        }
        private class ViewSpaceNormalTexturePass : ScriptableRenderPass
        {
            private ViewSpaceNormalsTextureSettings normalsTextureSettings;
            
            private FilteringSettings filteringSettings;
            
            private FilteringSettings occluderFilteringSettings;
            
            private readonly List<ShaderTagId> shaderTagIdList = new()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("SRPDefaultLit"),
            };
            
            private readonly RTHandle normals;
            
            private readonly Material occludersMaterial;
            
            private readonly Material normalsMaterial;

            
            
            public ViewSpaceNormalTexturePass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, LayerMask occluderLayerMask, ViewSpaceNormalsTextureSettings settings)
            {
                this.renderPassEvent = renderPassEvent;
                normalsTextureSettings = settings; 
                
                filteringSettings = new FilteringSettings(RenderQueueRange.opaque, outlinesLayerMask);
                occluderFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, occluderLayerMask);
                

                normals = RTHandles.Alloc("_SceneViewSpaceNormals", name: "_SceneViewSpaceNormals");
                normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
                
                occludersMaterial = new Material(Shader.Find("Hidden/UnlitColor"));
                occludersMaterial.SetColor("_Color", normalsTextureSettings.backgroundColor);
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
                normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
                normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
                cmd.GetTemporaryRT(normals.GetInstanceID(), normalsTextureDescriptor, normalsTextureSettings.filterMode);
                
                ConfigureTarget(normals);
                ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!normalsMaterial || !occludersMaterial) return; 
                
                CommandBuffer cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsCreation")))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                    drawingSettings.perObjectData = normalsTextureSettings.perObjectData;
                    drawingSettings.enableDynamicBatching = normalsTextureSettings.enableDynamicBatching;
                    drawingSettings.enableInstancing = normalsTextureSettings.enableInstancing;
                    drawingSettings.overrideMaterial = normalsMaterial;

                    DrawingSettings occluderSettings = drawingSettings;
                    occluderSettings.overrideMaterial = occludersMaterial;
                    
                    RendererListParams rendererListParams =
                        new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams); 
                    cmd.DrawRendererList(rendererList);
                    
                    rendererListParams =
                        new RendererListParams(renderingData.cullResults, occluderSettings, occluderFilteringSettings);
                    rendererList = context.CreateRendererList(ref rendererListParams); 
                    cmd.DrawRendererList(rendererList);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(normals.GetInstanceID());
            }
        }

        private class ScreenSpaceOutlinePass : ScriptableRenderPass
        {

            private readonly Material screenSpaceOutlineMaterial; 
            
            private RTHandle cameraColorTarget; 
            
            private RTHandle temporaryBuffer;
            
            
            
            public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, ScreenSpaceOutlineSettings settings)
            {
                this.renderPassEvent = renderPassEvent;
                screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
                screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.outlineColor);
                screenSpaceOutlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);

                screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
                screenSpaceOutlineMaterial.SetFloat("_RobertsCrossMultiplier", settings.robertsCrossMultiplier);

                screenSpaceOutlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);

                screenSpaceOutlineMaterial.SetFloat("_SteepAngleThreshold", settings.steepAngleThreshold);
                screenSpaceOutlineMaterial.SetFloat("_SteepAngleMultiplier", settings.steepAngleMultiplier);
            }

          

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                temporaryTargetDescriptor.depthBufferBits = 0;
                RTHandles.Initialize(Screen.width, Screen.height);
                temporaryBuffer = RTHandles.Alloc("_TemporaryBuffer", "_TemporaryBuffer");
                cmd.GetTemporaryRT(temporaryBuffer.GetInstanceID(), temporaryTargetDescriptor, FilterMode.Bilinear);
                
                
                cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if(!screenSpaceOutlineMaterial) 
                    return;
                
                CommandBuffer cmd = CommandBufferPool.Get();
                
                using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
                {
                    Blit(cmd, cameraColorTarget, temporaryBuffer);
                    Blit(cmd, temporaryBuffer, cameraColorTarget, screenSpaceOutlineMaterial);
                }
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(temporaryBuffer.GetInstanceID());
            }
        }

       
        [SerializeField] private LayerMask outlineLayerMask;
        [SerializeField] private LayerMask outlinesOccluderLayerMask;
        [SerializeField] private RenderPassEvent renderPassEvent; 
        [SerializeField] private ViewSpaceNormalsTextureSettings settings = new ViewSpaceNormalsTextureSettings();
        [SerializeField] private ScreenSpaceOutlineSettings outlineSettings = new ScreenSpaceOutlineSettings();
        
        private ViewSpaceNormalTexturePass viewSpaceNormalTexturePass;
        private ScreenSpaceOutlinePass screenSpaceOutlinePass;
        public override void Create()
        {
            viewSpaceNormalTexturePass = new ViewSpaceNormalTexturePass(renderPassEvent, outlineLayerMask,outlinesOccluderLayerMask ,settings);
            screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlineSettings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
          renderer.EnqueuePass(viewSpaceNormalTexturePass);
          renderer.EnqueuePass(screenSpaceOutlinePass);
        }
    }
}
