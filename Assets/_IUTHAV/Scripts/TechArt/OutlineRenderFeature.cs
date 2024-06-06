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
        private class ViewSpaceNormalsTextureSettings
        {
            public int depthBufferBits = 16;
            public RenderTextureFormat colorFormat;
            public Color backgroundColor = Color.black;
        }
        private class ViewSpaceNormalTexturePass : ScriptableRenderPass
        {
            private ViewSpaceNormalsTextureSettings normalsTextureSettings;
            
            private FilteringSettings filteringSettings;
            
            private readonly RTHandle normals;
            
            private readonly Material normalsMaterial;

            private readonly List<ShaderTagId> shaderTagIdList = new()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("SRPDefaultLit"),
            };
            
            public ViewSpaceNormalTexturePass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, ViewSpaceNormalsTextureSettings settings)
            {
                this.renderPassEvent = renderPassEvent;
                normalsTextureSettings = settings; 
                
                filteringSettings = new FilteringSettings(RenderQueueRange.opaque, outlinesLayerMask);

                normals = RTHandles.Alloc("_SceneViewSpaceNormals", name: "_SceneViewSpaceNormals");
                normalsMaterial = new Material(Shader.Find("Shader Graphs/ViewSpaceNormalsShader"));
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
                normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
                normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
                cmd.GetTemporaryRT(normals.GetInstanceID(), normalsTextureDescriptor, FilterMode.Point);
                
                ConfigureTarget(normals);
                ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!normalsMaterial) return; 
                
                CommandBuffer cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsCreation")))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                    drawingSettings.overrideMaterial = normalsMaterial;


                    RendererListParams rendererListParams =
                        new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                    var rendererList = context.CreateRendererList(ref rendererListParams); 
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
            
            private int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");
            
            
            public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent)
            {
                this.renderPassEvent = renderPassEvent;
                screenSpaceOutlineMaterial = new Material(Shader.Find("Shader Graphs/OutlineShader"));
            }

          

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                temporaryTargetDescriptor.depthBufferBits = 0;
                cmd.GetTemporaryRT(temporaryBufferID, temporaryTargetDescriptor, FilterMode.Bilinear);
                temporaryBuffer = RTHandles.Alloc("_TemporaryBuffer", "_TemporaryBuffer");

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
                    Blit(cmd, cameraColorTarget, temporaryBuffer, screenSpaceOutlineMaterial);
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
        [SerializeField] private RenderPassEvent renderPassEvent; 
        [SerializeField] private ViewSpaceNormalsTextureSettings settings;
        
        private ViewSpaceNormalTexturePass viewSpaceNormalTexturePass;
        private ScreenSpaceOutlinePass screenSpaceOutlinePass;
        public override void Create()
        {
            viewSpaceNormalTexturePass = new ViewSpaceNormalTexturePass(renderPassEvent, outlineLayerMask, settings);
            screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
          renderer.EnqueuePass(viewSpaceNormalTexturePass);
          renderer.EnqueuePass(screenSpaceOutlinePass);
        }
    }
}
