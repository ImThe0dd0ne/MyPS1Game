Shader "Hidden/FogPlus"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            Name "FogPass"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_DistanceLUT);
            SAMPLER(sampler_DistanceLUT);
            TEXTURE2D(_HeightLUT);
            SAMPLER(sampler_HeightLUT);
            
            float _Near;
            float _Far;
            float _UseDistanceFog;
            float _UseDistanceFogOnSky;
            float _DistanceFogIntensity;
            
            float _LowWorldY;
            float _HighWorldY;
            float _UseHeightFog;
            float _UseHeightFogOnSky;
            float _HeightFogIntensity;
            
            float _DistanceHeightBlend;
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                
                // Calculate view vector for world position reconstruction
                float3 viewVector = mul(unity_CameraInvProjection, float4(input.uv * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0)).xyz;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Sample depth and convert to linear depth
                float rawDepth = SampleSceneDepth(input.uv);
                float depth = LinearEyeDepth(rawDepth, _ZBufferParams);
                
                // Check if this is skybox (far plane)
                bool isSky = rawDepth >= 0.999999;
                
                // Calculate world position
                float3 worldPos = _WorldSpaceCameraPos + input.viewVector * depth;
                
                // Initialize fog color to no fog (original color)
                half4 finalColor = color;
                
                // Distance fog
                if (_UseDistanceFog > 0.5 && (!isSky || _UseDistanceFogOnSky > 0.5))
                {
                    float distanceT = saturate((depth - _Near) / (_Far - _Near));
                    half4 distanceFog = SAMPLE_TEXTURE2D(_DistanceLUT, sampler_DistanceLUT, float2(distanceT, 0.5));
                    finalColor.rgb = lerp(finalColor.rgb, distanceFog.rgb, distanceFog.a * _DistanceFogIntensity);
                }
                
                // Height fog
                if (_UseHeightFog > 0.5 && (!isSky || _UseHeightFogOnSky > 0.5))
                {
                    float heightT = saturate((worldPos.y - _LowWorldY) / (_HighWorldY - _LowWorldY));
                    heightT = 1.0 - heightT; // Invert so fog is thicker at bottom
                    half4 heightFog = SAMPLE_TEXTURE2D(_HeightLUT, sampler_HeightLUT, float2(heightT, 0.5));
                    finalColor.rgb = lerp(finalColor.rgb, heightFog.rgb, heightFog.a * _HeightFogIntensity);
                }
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    Fallback Off
}