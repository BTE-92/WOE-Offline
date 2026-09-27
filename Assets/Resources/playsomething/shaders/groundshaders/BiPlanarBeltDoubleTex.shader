// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

Shader "PlaySomething/BiPlanarBeltDoubleTex" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Emission ("Emissive Color", Color) = (0,0,0,0)
        _Tiling ("Tiling", Float) = 1
        _TexBase1 ("Texture1 (Base)", 2D) = "white" {}
        _TexBase2 ("Texture2 (Base)", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ============================================================
        // FORWARD BASE PASS
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

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
            fixed4 _Emission;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                #ifdef LIGHTMAP_ON
                float4 texcoord1 : TEXCOORD1;
                #endif
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 objPos : TEXCOORD0;       // object space position offset
                float3 objNormal : TEXCOORD1;    // object space normal
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
                o.objPos = v.vertex.xyz + float3(0.0, 0.0, -225.0);
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

            // Unconditional bi-planar texturing (D3D9 SM3.0 compatible)
            half3 BiPlanarSample(float3 objPos, float3 objNormal, float tiling) {
                half2 projnormal = abs(normalize(objNormal.xy));
                projnormal = saturate(pow(projnormal, 6.0));

                float2 uv_xz = tiling * objPos.xz;
                float2 uv_yz = tiling * objPos.yz;

                // Unconditional samples to avoid D3D9 SM3.0 dynamic branching error X6077
                half3 color_xz1 = tex2D(_TexBase1, uv_xz).rgb;
                half3 color_xz2 = tex2D(_TexBase2, uv_xz).rgb;
                half3 color1    = tex2D(_TexBase2, uv_yz).rgb;

                half isPositiveY = step(0.0, objNormal.y);
                half3 color0     = lerp(color_xz2, color_xz1, isPositiveY);

                return lerp(color0, color1, projnormal.xxx);
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuse = BiPlanarSample(i.objPos, i.objNormal, _Tiling);

                half3 albedo = diffuse * _Color.rgb;
                half3 emissive = diffuse * _Emission.rgb;

                fixed4 c = fixed4(0,0,0,0);

                #ifdef LIGHTMAP_ON
                    // Lightmap path
                    fixed shadow = SHADOW_ATTENUATION(i);
                    half3 lm = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));

                    #if defined(SHADOWS_SCREEN) || defined(SHADOWS_NATIVE)
                        c.rgb = albedo * min(lm, shadow * 2.0);
                    #else
                        c.rgb = albedo * lm;
                    #endif
                #else
                    // Non-lightmap path
                    float3 worldN = normalize(i.worldNormal);
                    fixed shadow = SHADOW_ATTENUATION(i);
                    half NdotL = max(0, dot(worldN, _WorldSpaceLightPos0.xyz));
                    half3 directLight = albedo * _LightColor0.rgb * (NdotL * shadow * 2.0);
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
            Name "FORWARDADD"
            Tags { "LightMode"="ForwardAdd" }
            ZWrite Off
            Blend One One
            Fog { Color (0,0,0,0) }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            fixed4 _Emission;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;

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
                o.objPos = v.vertex.xyz + float3(0.0, 0.0, -225.0);
                o.objNormal = normalize(v.normal);

                float3 worldNormal = normalize(mul((float3x3)_Object2World, v.normal * unity_Scale.w));
                o.worldNormal = worldNormal;

                float3 worldPos = mul(_Object2World, v.vertex).xyz;

                // For directional lights _WorldSpaceLightPos0.w == 0
                // For point/spot lights _WorldSpaceLightPos0.w == 1
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            half3 BiPlanarSample(float3 objPos, float3 objNormal, float tiling) {
                half2 projnormal = abs(normalize(objNormal.xy));
                projnormal = saturate(pow(projnormal, 6.0));

                float2 uv_xz = tiling * objPos.xz;
                float2 uv_yz = tiling * objPos.yz;

                half3 color_xz1 = tex2D(_TexBase1, uv_xz).rgb;
                half3 color_xz2 = tex2D(_TexBase2, uv_xz).rgb;
                half3 color1    = tex2D(_TexBase2, uv_yz).rgb;

                half isPositiveY = step(0.0, objNormal.y);
                half3 color0     = lerp(color_xz2, color_xz1, isPositiveY);

                return lerp(color0, color1, projnormal.xxx);
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuse = BiPlanarSample(i.objPos, i.objNormal, _Tiling);
                half3 albedo = diffuse * _Color.rgb;

                float3 lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);
                half NdotL = max(0, dot(normalize(i.worldNormal), lightDir));

                fixed4 c;
                c.rgb = albedo * _LightColor0.rgb * (NdotL * atten * 2.0);
                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PREPASS BASE (Deferred Lighting - Normals)
        // ============================================================
        Pass {
            Name "PREPASSBASE"
            Tags { "LightMode"="PrePassBase" }
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
                float3 objPos : TEXCOORD0;
                float3 objNormal : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.objPos = v.vertex.xyz + float3(0.0, 0.0, -225.0);
                o.objNormal = normalize(v.normal);
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
            Name "PREPASSFINAL"
            Tags { "LightMode"="PrePassFinal" }
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
            fixed4 _Emission;
            float _Tiling;
            sampler2D _TexBase1;
            sampler2D _TexBase2;
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
                o.objPos = v.vertex.xyz + float3(0.0, 0.0, -225.0);
                o.objNormal = normalize(v.normal);

                // Screen-space UV for _LightBuffer
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

            half3 BiPlanarSample(float3 objPos, float3 objNormal, float tiling) {
                half2 projnormal = abs(normalize(objNormal.xy));
                projnormal = saturate(pow(projnormal, 6.0));

                float2 uv_xz = tiling * objPos.xz;
                float2 uv_yz = tiling * objPos.yz;

                half3 color_xz1 = tex2D(_TexBase1, uv_xz).rgb;
                half3 color_xz2 = tex2D(_TexBase2, uv_xz).rgb;
                half3 color1    = tex2D(_TexBase2, uv_yz).rgb;

                half isPositiveY = step(0.0, objNormal.y);
                half3 color0     = lerp(color_xz2, color_xz1, isPositiveY);

                return lerp(color0, color1, projnormal.xxx);
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuse = BiPlanarSample(i.objPos, i.objNormal, _Tiling);
                half3 albedo = diffuse * _Color.rgb;
                half3 emissive = diffuse * _Emission.rgb;

                // Sample light buffer
                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                    light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                    light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                #ifdef LIGHTMAP_ON
                    #if defined(DIRLIGHTMAP_ON)
                        // Directional lightmap
                        half3 lm = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));
                        half4 lmAdd;
                        lmAdd.w = 0.0;
                        lmAdd.xyz = lm;
                        #ifdef HDR_LIGHT_PREPASS_ON
                            light += lmAdd;
                        #else
                            light += lmAdd;
                        #endif
                    #else
                        // Non-directional lightmap with fade
                        float lmFade = sqrt(dot(i.fadeDist, i.fadeDist)) * unity_LightmapFade.z + unity_LightmapFade.w;
                        lmFade = saturate(lmFade);
                        half3 lmFull = 2.0 * DecodeLightmap(tex2D(unity_Lightmap, i.lmap));
                        half3 lmIndirect = 2.0 * DecodeLightmap(tex2D(unity_LightmapInd, i.lmap));
                        light.rgb += lerp(lmIndirect, lmFull, lmFade);
                    #endif
                #else
                    // SH ambient
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