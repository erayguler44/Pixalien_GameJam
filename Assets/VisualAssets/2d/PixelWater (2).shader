// Pixel art su shader'ı (URP).
// - Dokuyu tekrar ettirir (WaterScroll script'i ile akar)
// - Duvarlara / objelere değdiği yerde dalgalı, pikselli köpük çizgisi çizer
// - Kesişme kenarını yumuşatır
// - Yüzeyi hafifçe yükseltip alçaltır (sadece görsel)
// Not: Kameranın Depth Texture ayarı açık olmalı.
Shader "Custom/PixelWater"
{
    Properties
    {
        _BaseMap ("Su Dokusu", 2D) = "white" {}
        _BaseColor ("Renk / Saydamlık", Color) = (1, 1, 1, 0.8)

        [Header(Kopuk)]
        _FoamColor ("Köpük Rengi", Color) = (1, 1, 1, 0.9)
        _FoamWidth ("Köpük Kalınlığı", Range(0, 1)) = 0.12
        _FoamWaveAmount ("Köpük Dalga Miktarı", Range(0, 0.5)) = 0.06
        _FoamWaveFreq ("Köpük Dalga Sıklığı", Float) = 3
        _FoamWaveSpeed ("Köpük Dalga Hızı", Float) = 2
        _PixelDensity ("Piksel Yoğunluğu (birim başına)", Float) = 8
        _FlatThreshold ("Düz Zemin Eşiği", Range(0, 1)) = 0.7

        [Header(Kenar ve Hareket)]
        _EdgeFade ("Kenar Yumuşaklığı", Range(0.001, 1)) = 0.05
        _BobAmount ("Yükselip Alçalma Miktarı", Range(0, 0.2)) = 0.03
        _BobSpeed ("Yükselip Alçalma Hızı", Float) = 1.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "PixelWater"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half4  _FoamColor;
                float  _FoamWidth;
                float  _FoamWaveAmount;
                float  _FoamWaveFreq;
                float  _FoamWaveSpeed;
                float  _PixelDensity;
                float  _FlatThreshold;
                float  _EdgeFade;
                float  _BobAmount;
                float  _BobSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float4 screenPos  : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float  eyeDepth   : TEXCOORD3;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                posWS.y += sin(_Time.y * _BobSpeed) * _BobAmount;   // hafif nefes alma

                OUT.positionWS = posWS;
                OUT.positionCS = TransformWorldToHClip(posWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // Ekran koordinatı (derinlik dokusunu okumak için)
                float4 ndc = OUT.positionCS * 0.5f;
                OUT.screenPos.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
                OUT.screenPos.zw = OUT.positionCS.zw;

                OUT.eyeDepth = -TransformWorldToView(posWS).z;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                // Suyun arkasında görünen noktanın dünyadaki konumunu bul
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                #if !UNITY_REVERSED_Z
                    rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, rawDepth);
                #endif
                float3 sceneWS = ComputeWorldSpacePosition(screenUV, rawDepth, UNITY_MATRIX_I_VP);

                // Su yüzeyi ile o nokta arasındaki YÜKSEKLİK farkı:
                // duvar/çöp kutusu su yüzeyini kestiği yerde 0'a yaklaşır,
                // altta kalan zeminde ise su derinliği kadardır.
                float diff = IN.positionWS.y - sceneWS.y;

                // Pikselli, dalgalı köpük çizgisi
                float px = floor(IN.positionWS.x * _PixelDensity) / _PixelDensity;
                float wave = sin(px * _FoamWaveFreq + _Time.y * _FoamWaveSpeed) * _FoamWaveAmount;
                float foam = step(diff, _FoamWidth + wave);

                // Arkadaki yüzeyin yönünü bul: düz zeminse (yol, kaldırım) köpük çizme,
                // sadece dik yüzeylerde (duvar, çöp kutusu) çiz.
                float3 n = cross(ddy(sceneWS), ddx(sceneWS));
                n = normalize(n + float3(0, 1e-6, 0));
                float steep = 1.0 - saturate((abs(n.y) - _FlatThreshold) / 0.1);
                foam *= steep;

                col.rgb = lerp(col.rgb, _FoamColor.rgb, foam * _FoamColor.a);
                col.a = max(col.a, foam * _FoamColor.a);

                // Kesişme kenarını yumuşat
                col.a *= saturate(diff / _EdgeFade);

                return col;
            }
            ENDHLSL
        }
    }
}
