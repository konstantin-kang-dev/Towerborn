Shader "Custom/SimpleTreeWind"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _WindStrength ("Wind Strength", Range(0, 2)) = 0.5
        _WindSpeed ("Wind Speed", Range(0, 10)) = 1.0
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0, 0)
        _RandomOffset ("Random Offset", Range(0, 10)) = 1.0
        _NoiseScale ("Noise Scale", Range(0.1, 5)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float fogCoord : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _WindStrength;
                float _WindSpeed;
                float3 _WindDirection;
                float _RandomOffset;
                float _NoiseScale;
            CBUFFER_END
            
            // Простая функция шума для рандомизации
            float SimpleNoise(float3 pos)
            {
                return frac(sin(dot(pos, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            
            // Плавный шум на основе позиции
            float SmoothNoise(float3 pos)
            {
                float3 i = floor(pos);
                float3 f = frac(pos);
                f = f * f * (3.0 - 2.0 * f);
                
                float n = i.x + i.y * 157.0 + 113.0 * i.z;
                
                float a = SimpleNoise(float3(n, n + 1.0, n + 2.0));
                float b = SimpleNoise(float3(n + 157.0, n + 158.0, n + 159.0));
                
                return lerp(a, b, f.z);
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Получаем мировую позицию для уникальности каждого дерева
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                
                // Используем позицию объекта для создания уникального оффсета
                float3 objectOrigin = TransformObjectToWorld(float3(0,0,0));
                float uniqueOffset = SimpleNoise(objectOrigin * _RandomOffset);
                
                // Время с уникальным смещением для каждого дерева
                float windTime = _Time.y * _WindSpeed + uniqueOffset * 6.28;
                
                // Используем vertex color для определения силы изгиба
                float bendFactor = input.color.r;
                
                // Создаем несколько волн с разными частотами
                float wave1 = sin(worldPos.x * 0.1 + windTime) * 0.5 + 0.5;
                float wave2 = sin(worldPos.z * 0.15 + windTime * 0.7) * 0.5;
                float wave3 = sin(worldPos.x * 0.05 + worldPos.z * 0.05 + windTime * 1.3) * 0.3;
                
                // Добавляем шум для микро-вариаций
                float noise = SmoothNoise(worldPos * _NoiseScale + float3(windTime * 0.1, 0, windTime * 0.1));
                
                // Комбинируем все волны
                float totalWave = (wave1 + wave2 + wave3 + noise * 0.5) * 0.5;
                
                // Применяем смещение с учетом уникальности дерева
                float3 windOffset = _WindDirection * totalWave * _WindStrength * bendFactor;
                
                // Добавляем небольшое вращательное движение
                float rotationWave = sin(windTime * 0.8 + uniqueOffset * 3.14) * 0.1;
                windOffset.x += rotationWave * bendFactor * _WindStrength * 0.5;
                
                input.positionOS.xyz += windOffset;
                
                // Стандартные преобразования
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.fogCoord = ComputeFogFactor(positionInputs.positionCS.z);
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float4 color = texColor * _Color;
                
                // Простое освещение
                Light mainLight = GetMainLight();
                float3 normalWS = normalize(input.normalWS);
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 lighting = mainLight.color * NdotL + float3(0.2, 0.2, 0.2);
                
                color.rgb *= lighting;
                color.rgb = MixFog(color.rgb, input.fogCoord);
                
                return color;
            }
            ENDHLSL
        }
        
        // Shadow Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            float3 _LightDirection;
            
            // Те же переменные для анимации
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _WindStrength;
                float _WindSpeed;
                float3 _WindDirection;
                float _RandomOffset;
                float _NoiseScale;
            CBUFFER_END
            
            // Копируем функции шума
            float SimpleNoise(float3 pos)
            {
                return frac(sin(dot(pos, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            
            float SmoothNoise(float3 pos)
            {
                float3 i = floor(pos);
                float3 f = frac(pos);
                f = f * f * (3.0 - 2.0 * f);
                
                float n = i.x + i.y * 157.0 + 113.0 * i.z;
                
                float a = SimpleNoise(float3(n, n + 1.0, n + 2.0));
                float b = SimpleNoise(float3(n + 157.0, n + 158.0, n + 159.0));
                
                return lerp(a, b, f.z);
            }
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                
                // Применяем ту же анимацию ветра, что и в основном проходе
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 objectOrigin = TransformObjectToWorld(float3(0,0,0));
                float uniqueOffset = SimpleNoise(objectOrigin * _RandomOffset);
                float windTime = _Time.y * _WindSpeed + uniqueOffset * 6.28;
                float bendFactor = input.color.r;
                
                float wave1 = sin(worldPos.x * 0.1 + windTime) * 0.5 + 0.5;
                float wave2 = sin(worldPos.z * 0.15 + windTime * 0.7) * 0.5;
                float wave3 = sin(worldPos.x * 0.05 + worldPos.z * 0.05 + windTime * 1.3) * 0.3;
                float noise = SmoothNoise(worldPos * _NoiseScale + float3(windTime * 0.1, 0, windTime * 0.1));
                float totalWave = (wave1 + wave2 + wave3 + noise * 0.5) * 0.5;
                
                float3 windOffset = _WindDirection * totalWave * _WindStrength * bendFactor;
                float rotationWave = sin(windTime * 0.8 + uniqueOffset * 3.14) * 0.1;
                windOffset.x += rotationWave * bendFactor * _WindStrength * 0.5;
                
                input.positionOS.xyz += windOffset;
                
                // Применяем shadow bias
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(TransformObjectToWorld(input.positionOS.xyz), 
                                                                          TransformObjectToWorldNormal(input.normalOS), 
                                                                          _LightDirection));
                output.positionCS = positionCS;
                
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        // Depth Pass (для прозрачности и правильной сортировки)
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _WindStrength;
                float _WindSpeed;
                float3 _WindDirection;
                float _RandomOffset;
                float _NoiseScale;
            CBUFFER_END
            
            // Снова копируем функции для анимации
            float SimpleNoise(float3 pos)
            {
                return frac(sin(dot(pos, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }
            
            float SmoothNoise(float3 pos)
            {
                float3 i = floor(pos);
                float3 f = frac(pos);
                f = f * f * (3.0 - 2.0 * f);
                
                float n = i.x + i.y * 157.0 + 113.0 * i.z;
                
                float a = SimpleNoise(float3(n, n + 1.0, n + 2.0));
                float b = SimpleNoise(float3(n + 157.0, n + 158.0, n + 159.0));
                
                return lerp(a, b, f.z);
            }
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                
                // Та же анимация
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 objectOrigin = TransformObjectToWorld(float3(0,0,0));
                float uniqueOffset = SimpleNoise(objectOrigin * _RandomOffset);
                float windTime = _Time.y * _WindSpeed + uniqueOffset * 6.28;
                float bendFactor = input.color.r;
                
                float wave1 = sin(worldPos.x * 0.1 + windTime) * 0.5 + 0.5;
                float wave2 = sin(worldPos.z * 0.15 + windTime * 0.7) * 0.5;
                float wave3 = sin(worldPos.x * 0.05 + worldPos.z * 0.05 + windTime * 1.3) * 0.3;
                float noise = SmoothNoise(worldPos * _NoiseScale + float3(windTime * 0.1, 0, windTime * 0.1));
                float totalWave = (wave1 + wave2 + wave3 + noise * 0.5) * 0.5;
                
                float3 windOffset = _WindDirection * totalWave * _WindStrength * bendFactor;
                float rotationWave = sin(windTime * 0.8 + uniqueOffset * 3.14) * 0.1;
                windOffset.x += rotationWave * bendFactor * _WindStrength * 0.5;
                
                input.positionOS.xyz += windOffset;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
}