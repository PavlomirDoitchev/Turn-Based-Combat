// Shader "Custom/URP_GlowHighlight"
// {
//     Properties
//     {
//         _MainTex("Texture", 2D) = "white" {}
//         _BaseColor("Base Color", Color) = (0.3, 0.6, 1, 0.6)
//         _GlowColor("Glow Color", Color) = (0.1, 0.4, 1, 1)
//         _GlowStrength("Glow Strength", Float) = 1.5
//         _GlowPulseSpeed("Pulse Speed", Float) = 2.0
//     }

//     SubShader
//     {
//         Tags{"RenderType"="Transparent" "Queue"="Transparent"}
//         LOD 100

//         Pass
//         {
//             Blend SrcAlpha OneMinusSrcAlpha
//             ZWrite Off

//             HLSLPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile_fog

//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//             struct Attributes
//             {
//                 float4 positionOS   : POSITION;
//                 float2 uv           : TEXCOORD0;
//             };

//             struct Varyings
//             {
//                 float4 positionHCS  : SV_POSITION;
//                 float2 uv           : TEXCOORD0;
//             };

//             TEXTURE2D(_MainTex);
//             SAMPLER(sampler_MainTex);

//             float4 _BaseColor;
//             float4 _GlowColor;
//             float _GlowStrength;
//             float _GlowPulseSpeed;

//             Varyings vert(Attributes IN)
//             {
//                 Varyings OUT;
//                 OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
//                 OUT.uv = IN.uv;
//                 return OUT;
//             }

//             float4 frag(Varyings IN) : SV_Target
//             {
//                 float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

//                 // pulsating glow intensity
//                 float pulse = (sin(_Time.y * _GlowPulseSpeed) * 0.5 + 0.5);

//                 float4 glow = _GlowColor * pulse * _GlowStrength;

//                 float4 finalColor = tex * _BaseColor + glow;

//                 return finalColor;
//             }
//             ENDHLSL
//         }
//     }
// }
Shader "Custom/URP_GlowHighlight_Hover"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (0.3, 0.6, 1, 0.6)
        _GlowColor("Glow Color", Color) = (0.1, 0.4, 1, 1)
        _GlowStrength("Glow Strength", Float) = 1.5
        _GlowPulseSpeed("Pulse Speed", Float) = 2.0
        _HoveredQuad("Hovered Quad Index", Int) = -1
        _HoverBoost("Hover Brightness Boost", Float) = 1.8
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;

                uint vertexID       : SV_VertexID;    
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;

                uint vertexID       : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _BaseColor;
            float4 _GlowColor;
            float _GlowStrength;
            float _GlowPulseSpeed;
            int _HoveredQuad;
            float _HoverBoost;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                OUT.vertexID = IN.vertexID;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Pulsating glow intensity
                float pulse = (sin(_Time.y * _GlowPulseSpeed) * 0.5 + 0.5);
                float4 glow = _GlowColor * pulse * _GlowStrength;

                // Quad index = vertexID / 4
                int quadIndex = IN.vertexID / 4;

                // Apply hover highlight only to hovered quad
                float hoverFactor = (quadIndex == _HoveredQuad) ? _HoverBoost : 1.0;

                float4 finalColor = tex * _BaseColor * hoverFactor + glow;

                return finalColor;
            }
            ENDHLSL
        }
    }
}

