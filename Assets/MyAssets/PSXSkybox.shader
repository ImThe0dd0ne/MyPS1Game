Shader "Custom/PSXSkyboxDarkFantasy"
{
    Properties
    {
        _Cubemap ("Skybox Cubemap", Cube) = "" {}
        _TintColor ("Tint Color", Color) = (0.8, 0.7, 0.9, 1)
        _Exposure ("Exposure", Range(0, 8)) = 1
        _ColorSteps ("Color Steps", Range(4, 64)) = 16
        
        [Header(Dark Fantasy Effects)]
        _DarkenAmount ("Darken Amount", Range(0, 1)) = 0.3
        _ContrastBoost ("Contrast", Range(0.5, 3)) = 1.5
        _DesaturationAmount ("Desaturation", Range(0, 1)) = 0.4
        _VignetteStrength ("Vignette Strength", Range(0, 2)) = 0.8
        _VignetteSize ("Vignette Size", Range(0.1, 2)) = 1.2
        
        [Header(PSX Dithering)]
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.5
        _DitherScale ("Dither Scale", Range(1, 10)) = 4
        
        [Header(Atmospheric)]
        _FogTint ("Fog Tint", Color) = (0.3, 0.2, 0.4, 1)
        _FogDensity ("Fog Density", Range(0, 1)) = 0.2
        _HorizonFade ("Horizon Fade", Range(0, 2)) = 0.5
        
        [Header(Color Grading)]
        _Shadows ("Shadows Tint", Color) = (0.2, 0.15, 0.3, 1)
        _Midtones ("Midtones Tint", Color) = (0.6, 0.5, 0.7, 1)
        _Highlights ("Highlights Tint", Color) = (0.9, 0.8, 1, 1)
    }
    
    SubShader
    {
        Tags { 
            "Queue" = "Background" 
            "RenderType" = "Background" 
            "PreviewType" = "Skybox"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Cull Off 
        ZWrite Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 screenPos : TEXCOORD2;
            };
            
            TEXTURECUBE(_Cubemap);
            SAMPLER(sampler_Cubemap);
            
            half4 _TintColor;
            float _Exposure;
            float _ColorSteps;
            float _DarkenAmount;
            float _ContrastBoost;
            float _DesaturationAmount;
            float _VignetteStrength;
            float _VignetteSize;
            float _DitherStrength;
            float _DitherScale;
            half4 _FogTint;
            float _FogDensity;
            float _HorizonFade;
            half4 _Shadows;
            half4 _Midtones;
            half4 _Highlights;
            
            // PSX-style dither pattern
            float DitherPattern(float2 screenPos)
            {
                float2 ditherCoord = screenPos * _DitherScale;
                
                // 4x4 Bayer matrix for PSX-style dithering
                float4x4 bayerMatrix = float4x4(
                    0,  8,  2, 10,
                    12, 4, 14,  6,
                    3, 11,  1,  9,
                    15, 7, 13,  5
                ) / 16.0;
                
                int x = int(ditherCoord.x) % 4;
                int y = int(ditherCoord.y) % 4;
                return bayerMatrix[y][x];
            }
            
            // Color grading based on luminance
            half3 ColorGrade(half3 color)
            {
                float luma = dot(color, float3(0.299, 0.587, 0.114));
                
                // Determine shadow/midtone/highlight weights
                float shadowWeight = saturate(1.0 - smoothstep(0.0, 0.4, luma));
                float highlightWeight = saturate(smoothstep(0.6, 1.0, luma));
                float midtoneWeight = 1.0 - shadowWeight - highlightWeight;
                
                // Apply color grading
                half3 graded = color * (
                    _Shadows.rgb * shadowWeight +
                    _Midtones.rgb * midtoneWeight +
                    _Highlights.rgb * highlightWeight
                );
                
                return graded;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                float3 rotated = mul((float3x3)unity_ObjectToWorld, input.positionOS.xyz);
                output.positionHCS = mul(UNITY_MATRIX_VP, float4(rotated, 1.0));
                output.texcoord = input.positionOS.xyz;
                output.worldPos = rotated;
                
                // Calculate screen position for dithering
                output.screenPos = ComputeScreenPos(output.positionHCS).xy;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample the cubemap
                half4 col = SAMPLE_TEXTURECUBE(_Cubemap, sampler_Cubemap, input.texcoord);
                
                // Apply exposure (multiply by exposure value)
                col.rgb *= _Exposure;
                
                // Apply tint color
                col.rgb *= _TintColor.rgb;
                
                // Simple PSX color quantization (keep it simple first)
                col.rgb = floor(col.rgb * _ColorSteps) / _ColorSteps;
                
                return col;
            }
            ENDHLSL
        }
    }
    
    FallBack Off
}