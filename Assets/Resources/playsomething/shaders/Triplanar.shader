// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

Shader "PlaySomething/Triplanar" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Tiling ("Tiling", Float) = 1
        _TexBase1 ("Texture1 (Base)", 2D) = "white" {}
        _TexBase2 ("Texture2 (Base)", 2D) = "white" {}
        _TexBase3 ("Texture3 (Base)", 2D) = "white" {}
    }

    SubShader { 
        LOD 400
        Tags { "RenderType"="Opaque" }

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            float4 unity_LightmapST;
            #endif

            fixed4 _Color;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;
            sampler2D _TexBase3;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                #ifdef LIGHTMAP_ON
                float4 texcoord1 : TEXCOORD1;
                #endif
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 objNormal : TEXCOORD1;
                #ifdef LIGHTMAP_ON
                float2 lmap : TEXCOORD2;
                #else
                float3 worldNormal : TEXCOORD2;
                float3 shLight : TEXCOORD3;
                #endif
                SHADOW_COORDS(4)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);

                #ifdef LIGHTMAP_ON
                o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                float3 worldNormal = normalize(mul((float3x3)_Object2World, v.normal * unity_Scale.w));
                o.worldNormal = worldNormal;
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            half3 TriplanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                float2 uv_xy = uvScale * objPos.xy;
                float2 uv_zx = uvScale * objPos.zx;
                float2 uv_zy = uvScale * objPos.zy;

                half4 color0 = tex2D(_TexBase1, uv_xy); // XY plane texture
                half4 color1 = tex2D(_TexBase2, uv_zx); // ZX plane texture
                half4 color2 = tex2D(_TexBase3, uv_zy); // ZY plane texture

                // Normal-based blending weights matching the exact GLSL clamp/pow math
                half3 blend = saturate(pow(abs(objNormal) * 1.5, 50.0));

                // Blends Y and Z using Z weight, then interpolates towards X using X weight
                half4 color_YZ = lerp(color1, color0, blend.zzzz);
                half3 finalColor = lerp(color_YZ, color2, blend.xxxx).rgb;

                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = TriplanarSample(i.objPos, i.objNormal);
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 c = fixed4(0,0,0,0);

                #ifdef LIGHTMAP_ON
                half3 lm = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));
                #if defined(SHADOWS_SCREEN) || defined(SHADOWS_NATIVE)
                c.rgb = diffuseColor * min(lm, shadow * 2.0);
                #else
                c.rgb = diffuseColor * lm;
                #endif
                #else
                float3 worldNormal = normalize(i.worldNormal);
                half NdotL = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 directLight = diffuseColor * _LightColor0.rgb * (NdotL * shadow * 2.0);
                half3 indirectLight = diffuseColor * i.shLight;
                c.rgb = directLight + indirectLight;
                #endif

                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;
            sampler2D _TexBase3;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 objNormal : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 lightDir : TEXCOORD3;
                LIGHTING_COORDS(4, 5)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);

                float3 worldNormal = normalize(mul((float3x3)_Object2World, v.normal * unity_Scale.w));
                o.worldNormal = worldNormal;

                float3 worldPos = mul(_Object2World, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            half3 TriplanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                float2 uv_xy = uvScale * objPos.xy;
                float2 uv_zx = uvScale * objPos.zx;
                float2 uv_zy = uvScale * objPos.zy;

                half4 color0 = tex2D(_TexBase1, uv_xy);
                half4 color1 = tex2D(_TexBase2, uv_zx);
                half4 color2 = tex2D(_TexBase3, uv_zy);

                half3 blend = saturate(pow(abs(objNormal) * 1.5, 50.0));

                half4 color_YZ = lerp(color1, color0, blend.zzzz);
                half3 finalColor = lerp(color_YZ, color2, blend.xxxx).rgb;

                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = TriplanarSample(i.objPos, i.objNormal);

                float3 lightDir = normalize(i.lightDir);
                half NdotL = max(0.0, dot(normalize(i.worldNormal), lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 c;
                c.rgb = diffuseColor * _LightColor0.rgb * (NdotL * atten * 2.0);
                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PREPASS BASE (Deferred Lighting - Normals)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.worldNormal = normalize(mul((float3x3)_Object2World, v.normal * unity_Scale.w));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 res;
                res.xyz = i.worldNormal * 0.5 + 0.5;
                res.w = 0.0;
                return res;
            }
            ENDCG
        }

        // ============================================================
        // PREPASS FINAL (Deferred Lighting - Combine)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" "RenderType"="Opaque" }
            ZWrite Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
            #pragma multi_compile HDR_LIGHT_PREPASS_OFF HDR_LIGHT_PREPASS_ON

            #include "UnityCG.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            sampler2D unity_LightmapInd;
            float4 unity_LightmapST;
            float4 unity_LightmapFade;
            // float4 unity_ShadowFadeCenterAndType;
            #endif

            fixed4 _Color;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;
            sampler2D _TexBase3;
            sampler2D _LightBuffer;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                #ifdef LIGHTMAP_ON
                float4 texcoord1 : TEXCOORD1;
                #endif
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;
                float3 objNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                #ifdef LIGHTMAP_ON
                float2 lmap : TEXCOORD3;
                #if !defined(DIRLIGHTMAP_ON)
                float4 fadeDist : TEXCOORD4;
                #endif
                #else
                float3 shLight : TEXCOORD3;
                #endif
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);
                o.screenPos = ComputeScreenPos(o.pos);

                #ifdef LIGHTMAP_ON
                o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #if !defined(DIRLIGHTMAP_ON)
                float3 worldPos = mul(_Object2World, v.vertex).xyz;
                o.fadeDist.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
                o.fadeDist.w = -mul(UNITY_MATRIX_MV, v.vertex).z * (1.0 - unity_ShadowFadeCenterAndType.w);
                #endif
                #else
                float3 worldNormal = normalize(mul((float3x3)_Object2World, v.normal * unity_Scale.w));
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                return o;
            }

            half3 TriplanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                float2 uv_xy = uvScale * objPos.xy;
                float2 uv_zx = uvScale * objPos.zx;
                float2 uv_zy = uvScale * objPos.zy;

                half4 color0 = tex2D(_TexBase1, uv_xy);
                half4 color1 = tex2D(_TexBase2, uv_zx);
                half4 color2 = tex2D(_TexBase3, uv_zy);

                half3 blend = saturate(pow(abs(objNormal) * 1.5, 50.0));

                half4 color_YZ = lerp(color1, color0, blend.zzzz);
                half3 finalColor = lerp(color_YZ, color2, blend.xxxx).rgb;

                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = TriplanarSample(i.objPos, i.objNormal);
                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                #ifdef LIGHTMAP_ON
                #if defined(DIRLIGHTMAP_ON)
                half3 lm = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));
                light.rgb += lm;
                #else
                float lmFade = sqrt(dot(i.fadeDist, i.fadeDist)) * unity_LightmapFade.z + unity_LightmapFade.w;
                lmFade = saturate(lmFade);
                half3 lmFull = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));
                half3 lmIndirect = 2.0 * DecodeLightmap(tex2D(unity_LightmapInd, i.lmap));
                light.rgb += lerp(lmIndirect, lmFull, lmFade);
                #endif
                #else
                light.rgb += i.shLight;
                #endif

                fixed4 c;
                c.rgb = diffuseColor * light.rgb;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}