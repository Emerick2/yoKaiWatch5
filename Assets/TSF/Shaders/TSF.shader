Shader "TSF/Base1"
{
    Properties
    {
        [Toggle(_TEX_ON)] _DetailTex ("Enable Detail texture", Float) = 0
        _MainTex ("Detail", 2D) = "white" {}
        _ToonShade ("Shade", 2D) = "white" {}
        [Toggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0
        _Color ("Base Color", Color) = (1,1,1,1)
        [Toggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0
        _Brightness ("Brightness 1 = neutral", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        LOD 250
        ZWrite On
        Cull Back

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local _TEX_ON
            #pragma shader_feature_local _COLOR_ON
            #pragma shader_feature_local _VCOLOR_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_ToonShade);
            SAMPLER(sampler_ToonShade);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ToonShade_ST;
                half4 _Color;
                half _Brightness;
                half _DetailTex;
                half _TintColor;
                half _VertexColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                half3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 shadeUV : TEXCOORD0;

                #if defined(_TEX_ON)
                    float2 detailUV : TEXCOORD1;
                #endif

                #if defined(_VCOLOR_ON)
                    half4 color : COLOR;
                #endif
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Reproduce the original shader's view-space normal lookup.
                half3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                half3 normalVS = TransformWorldToViewDir(normalWS, true);
                OUT.shadeUV = normalVS.xy * 0.5h + 0.5h;

                #if defined(_TEX_ON)
                    OUT.detailUV = TRANSFORM_TEX(IN.uv, _MainTex);
                #endif

                #if defined(_VCOLOR_ON)
                    OUT.color = IN.color;
                #endif

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 result = SAMPLE_TEXTURE2D(_ToonShade, sampler_ToonShade, IN.shadeUV);

                #if defined(_COLOR_ON)
                    result *= _Color;
                #endif

                #if defined(_TEX_ON)
                    result *= SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.detailUV);
                #endif

                #if defined(_VCOLOR_ON)
                    result *= IN.color;
                #endif

                result *= _Brightness;
                return result;
            }
            ENDHLSL
        }
    }

    Fallback Off
}
