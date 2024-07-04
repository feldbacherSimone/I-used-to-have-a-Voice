Shader "Custom/URPRainShader"
{
    Properties
    {
        _MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
        _TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
        _PointSpotLightMultiplier ("Point/Spot Light Multiplier", Range (0, 10)) = 2
        _DirectionalLightMultiplier ("Directional Light Multiplier", Range (0, 10)) = 1
        _InvFade ("Soft Particles Factor", Range(0.01, 100.0)) = 1.0
        _AmbientLightMultiplier ("Ambient light multiplier", Range (0, 1)) = 0.25
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma multi_compile _ _SOFTPARTICLES_ON
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 _TintColor;
            half _DirectionalLightMultiplier;
            half _PointSpotLightMultiplier;
            half _AmbientLightMultiplier;

            #if defined(_SOFTPARTICLES_ON)
            half _InvFade;
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 positionCS : SV_POSITION;
                #if defined(_SOFTPARTICLES_ON)
                float4 projPos : TEXCOORD1;
                #endif
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.color = input.color * _TintColor;

                #if defined(_SOFTPARTICLES_ON)
                output.projPos = TransformObjectToHClip(input.positionOS);
                #endif

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;

                #if defined(_SOFTPARTICLES_ON)
                float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.projPos.xy / input.projPos.w);
                float partZ = input.projPos.z / input.projPos.w;
                color.a *= saturate(_InvFade * (LinearEyeDepth(sceneZ) - partZ));
                #endif

                return color;
            }

            ENDHLSL
        }
    }

    Fallback "Particles/Alpha Blended"
}
