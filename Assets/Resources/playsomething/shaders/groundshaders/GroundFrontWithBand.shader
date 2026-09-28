Shader "PlaySomething/GroundFrontWithBand" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (0,0,0,0)
        _EdgeTreshold ("Edge Treshold", Range(0.01,1)) = 0.35
        _Emission ("Emissive Color", Color) = (0,0,0,0)
        _MainTex ("Diffuse Base", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;
            fixed4 _Emission;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0; // xy = Mask, zw = Main
                #ifdef LIGHTMAP_ON
                float2 lmap : TEXCOORD1;
                #else
                float3 worldNormal : TEXCOORD1;
                float3 shLight : TEXCOORD2;
                #endif
                SHADOW_COORDS(3)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord1, _MainTex);

                #ifdef LIGHTMAP_ON
                o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = worldNormal;
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Gather texture colors
                half3 mainColor = tex2D(_MainTex, i.uv.zw).rgb;
                half iMask = 1.0 - tex2D(_MaskTex, i.uv.xy).a;

                // 2. Compute edge band interpolation
                half lowerTreshold = _EdgeTreshold * 0.9;
                half edgeInterpolator = saturate((iMask - lowerTreshold) / (_EdgeTreshold - lowerTreshold));
                half3 finalColor = lerp(mainColor, _EdgeColor.rgb, edgeInterpolator);

                if (iMask > _EdgeTreshold) {
                    finalColor = _EdgeColor.rgb;
                }

                // 3. Final visual configuration
                half3 albedo = finalColor * _Color.rgb;
                half3 emissive = finalColor * _Emission.rgb;
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 c = fixed4(0,0,0,0);

                #ifdef LIGHTMAP_ON
                half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
                #if defined(SHADOWS_SCREEN) || defined(SHADOWS_NATIVE)
                c.rgb = albedo * min(lm, shadow);
                #else
                c.rgb = albedo * lm;
                #endif
                #else
                float3 worldNormal = normalize(i.worldNormal);
                half NdotL = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 directLight = albedo * _LightColor0.rgb * (NdotL * shadow);
                half3 indirectLight = albedo * i.shLight;
                c.rgb = directLight + indirectLight;
                #endif

                c.rgb += emissive;
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord1, _MainTex);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 mainColor = tex2D(_MainTex, i.uv.zw).rgb;
                half iMask = 1.0 - tex2D(_MaskTex, i.uv.xy).a;

                half lowerTreshold = _EdgeTreshold * 0.9;
                half edgeInterpolator = saturate((iMask - lowerTreshold) / (_EdgeTreshold - lowerTreshold));
                half3 finalColor = lerp(mainColor, _EdgeColor.rgb, edgeInterpolator);

                if (iMask > _EdgeTreshold) {
                    finalColor = _EdgeColor.rgb;
                }

                half3 albedo = finalColor * _Color.rgb;
                float3 lightDir = normalize(i.lightDir);
                half NdotL = max(0.0, dot(normalize(i.worldNormal), lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 c;
                c.rgb = albedo * _LightColor0.rgb * (NdotL * atten);
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
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
            #pragma multi_compile HDR_LIGHT_PREPASS_OFF HDR_LIGHT_PREPASS_ON

            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _EdgeTreshold;
            fixed4 _Emission;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            sampler2D _LightBuffer;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                #ifdef LIGHTMAP_ON
                float4 texcoord1 : TEXCOORD1;
                #endif
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                #ifdef LIGHTMAP_ON
                float2 lmap : TEXCOORD2;
                #if !defined(DIRLIGHTMAP_ON)
                float4 fadeDist : TEXCOORD3;
                #endif
                #else
                float3 shLight : TEXCOORD2;
                #endif
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);

                #ifdef LIGHTMAP_ON
                o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #if !defined(DIRLIGHTMAP_ON)
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.fadeDist.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
                o.fadeDist.w = -UnityObjectToViewPos(v.vertex).z * (1.0 - unity_ShadowFadeCenterAndType.w);
                #endif
                #else
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 mainColor = tex2D(_MainTex, i.uv.zw).rgb;
                half iMask = 1.0 - tex2D(_MaskTex, i.uv.xy).a;

                half lowerTreshold = _EdgeTreshold * 0.9;
                half edgeInterpolator = saturate((iMask - lowerTreshold) / (_EdgeTreshold - lowerTreshold));
                half3 finalColor = lerp(mainColor, _EdgeColor.rgb, edgeInterpolator);

                if (iMask > _EdgeTreshold) {
                    finalColor = _EdgeColor.rgb;
                }

                half3 albedo = finalColor * _Color.rgb;
                half3 emissive = finalColor * _Emission.rgb;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                #ifdef LIGHTMAP_ON
                #if defined(DIRLIGHTMAP_ON)
                half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
                light.rgb += lm;
                #else
                float lmFade = sqrt(dot(i.fadeDist, i.fadeDist)) * unity_LightmapFade.z + unity_LightmapFade.w;
                lmFade = saturate(lmFade);
                half3 lmFull = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
                half3 lmIndirect = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap, i.lmap));
                light.rgb += lerp(lmIndirect, lmFull, lmFade);
                #endif
                #else
                light.rgb += i.shLight;
                #endif

                fixed4 c;
                c.rgb = albedo * light.rgb + emissive;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}