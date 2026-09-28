Shader "PlaySomething/BiPlanarBeltSingleTex" {
    Properties {
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        _Tiling ("Tiling", Float) = 1
        _TexBase1 ("Texture1 (Base)", 2D) = "white" {}
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

            fixed4 _Color;
            float _Tiling;
            sampler2D _TexBase1;

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

                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);

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

            half3 BiPlanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                
                // Projection planes: XZ and YZ
                float2 tex0_uv = uvScale * objPos.xz;
                float2 tex1_uv = uvScale * objPos.yz;

                // Mirror V coordinates as dictated by GLSL: tex0_uv.y * -1.0
                tex0_uv.y *= -1.0;
                tex1_uv.y *= -1.0;

                half4 color0 = tex2D(_TexBase1, tex0_uv);
                half4 color1 = tex2D(_TexBase1, tex1_uv);

                // Normal-based blending weight from the absolute object-space normal X component
                half blend = saturate(pow(abs(objNormal.x) * 1.5, 50.0));

                half3 finalColor = lerp(color0, color1, blend).rgb;
                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = BiPlanarSample(i.objPos, i.objNormal);
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 c = fixed4(0,0,0,0);

                #ifdef LIGHTMAP_ON
                half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
                #if defined(SHADOWS_SCREEN) || defined(SHADOWS_NATIVE)
                c.rgb = diffuseColor * min(lm, shadow);
                #else
                c.rgb = diffuseColor * lm;
                #endif
                #else
                float3 worldNormal = normalize(i.worldNormal);
                half NdotL = max(0.0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half3 directLight = diffuseColor * _LightColor0.rgb * (NdotL * shadow);
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

                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            half3 BiPlanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                float2 tex0_uv = uvScale * objPos.xz;
                float2 tex1_uv = uvScale * objPos.yz;

                tex0_uv.y *= -1.0;
                tex1_uv.y *= -1.0;

                half4 color0 = tex2D(_TexBase1, tex0_uv);
                half4 color1 = tex2D(_TexBase1, tex1_uv);

                half blend = saturate(pow(abs(objNormal.x) * 1.5, 50.0));

                half3 finalColor = lerp(color0, color1, blend).rgb;
                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = BiPlanarSample(i.objPos, i.objNormal);

                float3 lightDir = normalize(i.lightDir);
                half NdotL = max(0.0, dot(normalize(i.worldNormal), lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 c;
                c.rgb = diffuseColor * _LightColor0.rgb * (NdotL * atten);
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
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
            #pragma multi_compile HDR_LIGHT_PREPASS_OFF HDR_LIGHT_PREPASS_ON

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Tiling;
            sampler2D _TexBase1;
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

                o.pos = UnityObjectToClipPos(v.vertex);
                o.objPos = v.vertex.xyz;
                o.objNormal = normalize(v.normal);
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

            half3 BiPlanarSample(float3 objPos, float3 objNormal) {
                float2 uvScale = _Tiling / 10.0;
                float2 tex0_uv = uvScale * objPos.xz;
                float2 tex1_uv = uvScale * objPos.yz;

                tex0_uv.y *= -1.0;
                tex1_uv.y *= -1.0;

                half4 color0 = tex2D(_TexBase1, tex0_uv);
                half4 color1 = tex2D(_TexBase1, tex1_uv);

                half blend = saturate(pow(abs(objNormal.x) * 1.5, 50.0));

                half3 finalColor = lerp(color0, color1, blend).rgb;
                return finalColor * _Color.rgb;
            }

            fixed4 frag (v2f i) : SV_Target {
                half3 diffuseColor = BiPlanarSample(i.objPos, i.objNormal);
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
                c.rgb = diffuseColor * light.rgb;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}