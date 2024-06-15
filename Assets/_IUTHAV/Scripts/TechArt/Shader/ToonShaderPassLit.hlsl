﻿#ifndef MY_TOON_SHADER_INCLUDE
#define MY_TOON_SHADER_INCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"



// See ShaderVariablesFunctions.hlsl in com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl
///////////////////////////////////////////////////////////////////////////////
//                      CBUFFER                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Unity URP requires us to set up a CBUFFER
(or "Constant Buffer") of Constant Variables.
These should be the same variables we set up 
in the Properties.
This CBUFFER is REQUIRED for Unity
to correctly handle per-material changes
as well as batching / instancing.
Don't skip it :)
*/
CBUFFER_START(UnityPerMaterial)
       TEXTURE2D(_ColorMap);
       SAMPLER(sampler_ColorMap);
       float4 _ColorMap_ST;
       float4 _Color;
       float _Smoothness;
       float _RimSharpness;
       float3 _RimColor;
       float _ShadowCutoff;
       float3 _AmbientColor;
       float _Specular;
       
CBUFFER_END
///////////////////////////////////////////////////////////////////////////////
//                      STRUCTS                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Our attributes struct is simple.
It contains the Object-Space Position
and Normal Direction as well as the 
UV0 coordinates for the mesh.
The Attributes struct is passed 
from the GPU to the Vertex function.
*/
struct Attributes
{
       float4 positionOS : POSITION;
       float3 normalOS   : NORMAL;
       float2 uv         : TEXCOORD0;
       
       // This line is required for VR SPI to work.
       UNITY_VERTEX_INPUT_INSTANCE_ID
};
/*
The Varyings struct is also straightforward.
It contains the Clip Space Position, the UV, and 
the World-Space Normals.
The Varyings struct is passed from the Vertex
function to the Fragment function.
*/
struct Varyings
{
       float4 positionHCS     : SV_POSITION;
       float2 uv              : TEXCOORD0;
       float3 positionWS      : TEXCOORD1;
       float3 normalWS        : TEXCOORD2;
       float3 viewDirectionWS : TEXCOORD3;
       
       // This line is required for VR SPI to work.
    UNITY_VERTEX_INPUT_INSTANCE_ID
       UNITY_VERTEX_OUTPUT_STEREO
};
///////////////////////////////////////////////////////////////////////////////
//                      Common Lighting Transforms                           //
///////////////////////////////////////////////////////////////////////////////
// This is a global variable, Unity sets it for us.
float3 _LightDirection;
/*
This is a simple lighting transformation.
Normally, we just return the WorldToHClip position.
During the Shadow Pass, we want to make sure that Shadow Bias is baked 
in to the shadow map. To accomplish this, we use the ApplyShadowBias
method to push the world-space positions in their normal direction by the bias amount.
We define SHADOW_CASTER_PASS during the setup for the Shadow Caster pass.
*/
float4 GetClipSpacePosition(float3 positionWS, float3 normalWS)
{
       #if defined(SHADOW_CASTER_PASS)
           float4 positionHCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
           
           #if UNITY_REVERSED_Z
               positionHCS.z = min(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
           #else
               positionHCS.z = max(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
           #endif
           
           return positionHCS;
       #endif
       
       return TransformWorldToHClip(positionWS);
}


/*
These two functions give us the shadow coordinates 
depending on whether screen shadows are enabled or not.
We have two methods here, one with two args (positionWS
and positionHCS), and one with just positionWS.
The two-arg method is faster when you have 
already calculated the positionHCS variable.
*/
float4 GetMainLightShadowCoord(float3 positionWS, float4 positionHCS)
{
       #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           return ComputeScreenPos(positionHCS);
       #else
           return TransformWorldToShadowCoord(positionWS);
       #endif
}
float4 GetMainLightShadowCoord(float3 PositionWS)
{
       #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           float4 clipPos = TransformWorldToHClip(PositionWS);
           return ComputeScreenPos(clipPos);
       #else
    return TransformWorldToShadowCoord(PositionWS);
       #endif
}
/*
This method gives us the main light as an out parameter.
The Light struct is defined in 
"Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl",
so you can reference it there for more details on its fields.
This version of the GetMainLight method doesn't account for Light Cookies.
To account for Light cookies, you need to add the following line to your shader pass:
#pragma multi_compile _ _LIGHT_COOKIES
and also call a different GetMainLight method:
GetMainLight(float4 shadowCoord, float3 positionWS, half4 shadowMask)
*/
void GetMainLightData(float3 PositionWS, out Light light)
{
       float4 shadowCoord = GetMainLightShadowCoord(PositionWS);
       light = GetMainLight(shadowCoord);
}
///////////////////////////////////////////////////////////////////////////////
//                      Functions                                            //
///////////////////////////////////////////////////////////////////////////////
/*
The Vertex function is responsible 
for generating and manipulating the 
data for each vertex of the mesh.
*/
Varyings Vertex(Attributes IN)
{
       Varyings OUT = (Varyings)0;
       
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
       
       
       // Set up each field of the Varyings struct, then return it.
       OUT.positionWS = mul(unity_ObjectToWorld, IN.positionOS).xyz;
       OUT.viewDirectionWS = GetWorldSpaceNormalizeViewDir(OUT.positionWS);
       OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
       OUT.positionHCS = GetClipSpacePosition(OUT.positionWS, OUT.normalWS);
       OUT.uv = TRANSFORM_TEX(IN.uv, _ColorMap);
       
       return OUT;
}
/*
The FragmentDepthOnly function is responsible 
for handling per-pixel shading during the 
DepthOnly and ShadowCaster passes.
*/
float FragmentDepthOnly(Varyings IN) : SV_Target
{
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
       return 0;
}
/*
The FragmentDepthNormalsOnly function is responsible 
for handling per-pixel shading during the 
DepthNormalsOnly pass. This pass is less common, but
can be required by some post-process effects such as SSAO.
*/
float4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target
{
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
       return float4(normalize(IN.normalWS), 0);
}

float EasySmoothStep(float min, float x)
{
       return smoothstep(min, min + 0.01, x);
}

float3 CalculateLighting(Varyings IN, Light light)
{
       float NoL = dot(IN.normalWS, light.direction);
       float toonLighting = EasySmoothStep(_ShadowCutoff, NoL); ;
       float3 halfVector = normalize(light.direction + IN.viewDirectionWS);
       float NoH = max(dot(IN.normalWS, halfVector), 0);

       float specularTerm = pow(NoH, _Smoothness * _Smoothness);
       specularTerm *= toonLighting;
       specularTerm = EasySmoothStep(0.01, specularTerm);

       float NoV = max(dot(IN.normalWS, IN.viewDirectionWS), 0);
       float rimTerm = pow(1.0 - NoV, _RimSharpness);

       rimTerm *= toonLighting;
       rimTerm = EasySmoothStep(0.01, rimTerm);
       float3 rimLighting = rimTerm * _RimColor;

       float3 surfaceColor = _Color * SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, IN.uv);
       float3 directionalLighting = toonLighting * light.color;

       float toonShadows = EasySmoothStep(0.5, light.shadowAttenuation);

       float3 specularLighting = _Specular > 0 ? specularTerm * light.color : 0;
       float3 finalLighting = float3(0, 0, 0);
       finalLighting += directionalLighting;
       finalLighting += specularLighting;
       finalLighting += rimLighting;
       finalLighting*= toonShadows;

       return surfaceColor * (finalLighting + _AmbientColor);
}
/*
The Fragment function is responsible 
for handling per-pixel shading during the Forward 
rendering pass. We use the ForwardOnly pass, so this works
by default in both Forward and Deferred paths.
*/
float3 Fragment(Varyings IN) : SV_Target
{
       UNITY_SETUP_INSTANCE_ID(IN);
       UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

       Light mainLight;
       GetMainLightData(IN.positionWS, mainLight);

       IN.normalWS = normalize(IN.normalWS);
       IN.viewDirectionWS = normalize(IN.viewDirectionWS);

       // Determine if the fragment is a backface
       bool isBackFace = dot(IN.normalWS, IN.viewDirectionWS) < 0.0;
       if (isBackFace)
       {
              IN.normalWS = -IN.normalWS;
       }

       float3 finalColor = float3(0, 0, 0);

       // Handle main directional light
       finalColor += CalculateLighting(IN, mainLight);

       // Handle additional lights
       uint lightCount = GetAdditionalLightsCount();
       for (uint i = 0; i < lightCount; ++i)
       {
              Light additionalLight = GetAdditionalLight(i, IN.positionWS);

              float3 lightDir = normalize(additionalLight.direction - IN.positionWS);
              float distance = length(additionalLight.direction - IN.positionWS);
              float attenuation = 1.0 / (1.0 + distance * distance); // simple distance attenuation

              additionalLight.direction = lightDir;
              additionalLight.color *= attenuation;

              finalColor += CalculateLighting(IN, additionalLight);
       }

       return finalColor;
}
       

#endif