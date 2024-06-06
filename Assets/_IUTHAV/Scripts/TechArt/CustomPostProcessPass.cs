/*
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _IUTHAV.Scripts.TechArt
{
    [System.Serializable]
    
    public class CustomPostProcessPass : ScriptableRenderPass
    {
        private readonly OutlineRenderFeature.PassSettings m_settings;

        
        private Material _mat;
        private int bufferId = Shader.PropertyToID("_BufferName");
        private RenderTargetIdentifier src, outlineRT;
        
        private int objectOutlineBuffer = Shader.PropertyToID("_ObjectOutlineBuffer");
        private RenderTargetIdentifier objectOutlineRT;

        private int temporaryCameraBuffer = Shader.PropertyToID("_TemporaryCameraBuffer");
        private RenderTargetIdentifier temporaryCameraRT; 

        public CustomPostProcessPass(OutlineRenderFeature.PassSettings settings, CustomPostProcessPass _clone)
        {
            m_settings = settings; 
            if (!_mat)
            {
                _mat = CoreUtils.CreateEngineMaterial("CustomPost/OutlineMat");
            }

            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing; 
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            
            cmd.GetTemporaryRT(bufferId, cameraTextureDescriptor, FilterMode.Bilinear);
            outlineRT = new RenderTargetIdentifier(bufferId);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            var target = renderingData.cameraData.renderer.cameraColorTargetHandle; 
            
            
            
            
            Blit(cmd, target, temporaryCameraRT, _mat, 1);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);CommandBuffer cmd = CommandBufferPool.Get();
                                                       cmd.Clear();
        }
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
            cmd.ReleaseTemporaryRT(bufferId);
        }
    }
}

*/