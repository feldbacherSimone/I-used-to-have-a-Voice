Shader "Custom/TestObjectShader"
{
    Properties
    {
        [MainTexture] _ColorMap ("Color Map", 2D) = "white" {}
        [MainColor] _Color ("Color", Color) = (0.91, 0.91, 0.38)
        [Smoothness] _Smoothness ("Smoothness", float) = 16.0
        [RimSharpness] _RimSharpness ("Rim Sharpness", float) = 1
        [HDR] _RimColor("Rim Color", Color) = (1.0, 1.0, 1.0) 
        _ShadowCutoff("Shadow Cutoff", float) = 0
        _AmbientColor("Ambient Color", color) = (0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UnversalPipeline"  }
        LOD 100
        
        Cull Back 
        ZWrite On 
        ZTest LEqual 
        ZClip Off

        Pass
        {
            
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForwardOnly"} 
            
            HLSLPROGRAM

            #pragma  multi_compile_fog
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

            // These #pragma directives set up Main Light Shadows.
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #pragma vertex Vertex
            #pragma fragment Fragment
            // make fog work

            #include "ToonShaderPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name  "ShadowCaster"    
            Tags {"LightMode" = "ShadowCaster"} 
            
            HLSLPROGRAM

            #define SHADOW_CASTER_PASS
            #pragma vertex Vertex

            #pragma  fragment FragmentDepthOnly

            #include "ToonShaderPass.hlsl"
            ENDHLSL
        }
         Pass
           {
               Name "DepthOnly"
               Tags {"LightMode" = "DepthOnly"}
               
               
               HLSLPROGRAM
               
               #pragma vertex Vertex
               
               // Our DepthOnly Pass and ShadowCaster pass
               // both use the FragmentDepthOnly function
               #pragma fragment FragmentDepthOnly
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
           
           Pass
           {
               Name "DepthNormalsOnly"
               Tags {"LightMode" = "DepthNormalsOnly"}
               
               
               HLSLPROGRAM
               
               #pragma vertex Vertex
               
               // And our DepthNormalsOnly pass uses our
               // Fragment function named FragmentDepthNormalsOnly.
               #pragma fragment FragmentDepthNormalsOnly
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
    }
}
