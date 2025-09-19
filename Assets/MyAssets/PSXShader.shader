Shader "Custom/PSXShaderUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        // PSX Effects
        _VertexJitter ("Vertex Jitter", Range(0, 0.02)) = 0.01
        _ColorSteps ("Color Steps", Range(8, 128)) = 32
        _UVSnap ("UV Snap", Range(0, 256)) = 64
        _AffineMapping ("Affine Mapping", Range(0, 1)) = 1.0
        
        // Brightness control (since we're unlit)
        _Brightness ("Brightness", Range(0, 3)) = 1.0
        
        // Optional fake lighting for depth
        _FakeLightDir ("Fake Light Direction", Vector) = (0.5, 0.8, 0.2, 0)
        _FakeLightStrength ("Fake Light Strength", Range(0, 1)) = 0.3
    }
    
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float fogFactor    : TEXCOORD2;
                float4 screenPos   : TEXCOORD3;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float4 _MainTex_ST;
            float4 _Color;
            float _VertexJitter;
            float _ColorSteps;
            float _UVSnap;
            float _AffineMapping;
            float _Brightness;
            float4 _FakeLightDir;
            float _FakeLightStrength;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // Transform to world space
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                
                // PSX vertex jitter (snapping vertices to grid)
                if (_VertexJitter > 0.001)
                {
                    worldPos = floor(worldPos / _VertexJitter) * _VertexJitter;
                }
                
                // Transform to clip space
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                
                // Store screen position for affine mapping
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                
                // UV coordinates
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                // Snap UVs for that PSX texture swimming effect
                if (_UVSnap > 0)
                {
                    OUT.uv = floor(OUT.uv * _UVSnap) / _UVSnap;
                }
                
                // World space normal for fake lighting
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                
                // Fog
                OUT.fogFactor = ComputeFogFactor(OUT.positionHCS.z);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                // Affine texture mapping (PSX didn't have perspective-correct texturing)
                float2 uv = IN.uv;
                if (_AffineMapping > 0)
                {
                    float2 affineUV = IN.uv / IN.positionHCS.w;
                    uv = lerp(uv, affineUV, _AffineMapping);
                }
                
                // Sample texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
                
                // Optional fake lighting for some depth (completely optional)
                if (_FakeLightStrength > 0)
                {
                    float3 normal = normalize(IN.normalWS);
                    float3 lightDir = normalize(_FakeLightDir.xyz);
                    float NdotL = saturate(dot(normal, lightDir));
                    
                    // Quantize the fake lighting
                    NdotL = floor(NdotL * 4) / 4;
                    
                    // Apply subtle fake lighting
                    float fakeLighting = lerp(1.0, NdotL + 0.5, _FakeLightStrength);
                    col.rgb *= fakeLighting;
                }
                
                // Apply brightness
                col.rgb *= _Brightness;
                
                // Color quantization (reduce to PS1's 15-bit color depth)
                col.rgb = floor(col.rgb * _ColorSteps) / _ColorSteps;
                
                // Dithering pattern for extra PSX authenticity
                float2 screenPos = IN.positionHCS.xy;
                float dither = frac(dot(float2(171.0, 231.0) / 71.0, screenPos));
                col.rgb += (dither - 0.5) * (1.0 / _ColorSteps);
                
                // Apply fog (still works with unlit)
                col.rgb = MixFog(col.rgb, IN.fogFactor);
                
                return col;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Unlit"
}