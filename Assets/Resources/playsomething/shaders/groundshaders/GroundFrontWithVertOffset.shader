// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

// Reconstructed from Unity 4.x iOS GLES disassembly.
//
// Structure notes (inferred from the disassembly):
//  - The shader draws TWO layers per object:
//      "BACK"  = ground layer, samples _BeltTex, no vertex offset, normal ZTest.
//      "FRONT" = overlay layer, samples _MainTex, offsets vertices in the
//                XY plane by (uv1 * _OffsetX, uv1 * _OffsetY), and always
//                renders on top via ZTest Always (avoids z-fighting with BACK).
//  - Both layers implement ForwardBase / ForwardAdd / PrePassBase / PrePassFinal
//    (legacy "Light Prepass" deferred lighting support). No ShadowCaster or
//    ShadowCollector pass exists in the source disassembly, so none is added here.
//  - UV0 is always transformed with _MainTex_ST (even on the BACK layer, which
//    samples _BeltTex) - this quirk is preserved exactly from the disassembly.
//  - UV1 (texcoord1) does double duty on the FRONT layer: it's the offset mask
//    AND, under LIGHTMAP_ON, is reused as the lightmap UV source - preserved
//    exactly as found.

Shader "PlaySomething/GroundFrontWithVertOffset"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Inner tex(RGB)", 2D) = "white" {}
        _BeltTex ("Belt tex(RGB)", 2D) = "white" {}
        _OffsetX ("Vertex X offset", Float) = 0
        _OffsetY ("Vertex Y offset", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }

        // =====================================================================
        // BACK LAYER (ground) - samples _BeltTex, no vertex offset
        // =====================================================================

        Pass
        {
            Name "FORWARDBASE_BACK"
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_baseBack
            #pragma fragment frag_baseBack
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            float4 unity_LightmapST;
            #endif

            struct appdata_baseBack
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_baseBack
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 vLight : TEXCOORD2;
                float2 lmap : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };

            fixed4 _Color;
            sampler2D _BeltTex;
            float4 _MainTex_ST;

            v2f_baseBack vert_baseBack(appdata_baseBack v)
            {
                v2f_baseBack o;
                UNITY_INITIALIZE_OUTPUT(v2f_baseBack, o);

                float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.worldNormal = worldNormal;

                float3 vLight = ShadeSH9(half4(worldNormal, 1.0));
                #ifdef VERTEXLIGHT_ON
                    float3 worldPos = mul(_Object2World, v.vertex).xyz;
                    vLight += Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                        unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal);
                #endif
                o.vLight = vLight;

                #ifdef LIGHTMAP_ON
                    o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_baseBack(v2f_baseBack i) : SV_Target
            {
                fixed4 c = tex2D(_BeltTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                fixed4 col;
                #ifdef LIGHTMAP_ON
                    fixed3 lm = tex2D(unity_Lightmap, i.lmap).rgb * 2.0;
                    col.rgb = albedo * lm;
                #else
                    fixed atten = LIGHT_ATTENUATION(i);
                    fixed3 diff = albedo * _LightColor0.rgb *
                        (max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz)) * atten * 2.0);
                    col.rgb = diff + albedo * i.vLight;
                #endif
                col.a = 0;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARDADD_BACK"
            Tags { "LightMode"="ForwardAdd" }
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_addBack
            #pragma fragment frag_addBack
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata_addBack
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_addBack
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };

            fixed4 _Color;
            sampler2D _BeltTex;
            float4 _MainTex_ST;

            v2f_addBack vert_addBack(appdata_addBack v)
            {
                v2f_addBack o;
                UNITY_INITIALIZE_OUTPUT(v2f_addBack, o);

                float3 worldPos = mul(_Object2World, v.vertex).xyz;
                float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.worldNormal = worldNormal;

                #ifdef USING_DIRECTIONAL_LIGHT
                    o.lightDir = _WorldSpaceLightPos0.xyz;
                #else
                    o.lightDir = _WorldSpaceLightPos0.xyz - worldPos;
                #endif

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_addBack(v2f_addBack i) : SV_Target
            {
                fixed4 c = tex2D(_BeltTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                fixed3 lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 col;
                col.rgb = albedo * _LightColor0.rgb *
                    (max(0, dot(i.worldNormal, lightDir)) * atten * 2.0);
                col.a = 0;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASSBASE_BACK"
            Tags { "LightMode"="PrePassBase" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepassBaseBack
            #pragma fragment frag_prepassBaseBack
            #include "UnityCG.cginc"

            struct appdata_prepassBaseBack
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f_prepassBaseBack
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f_prepassBaseBack vert_prepassBaseBack(appdata_prepassBaseBack v)
            {
                v2f_prepassBaseBack o;
                UNITY_INITIALIZE_OUTPUT(v2f_prepassBaseBack, o);
                o.worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            fixed4 frag_prepassBaseBack(v2f_prepassBaseBack i) : SV_Target
            {
                fixed4 res;
                res.rgb = i.worldNormal * 0.5 + 0.5;
                res.a = 0;
                return res;
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASSFINAL_BACK"
            Tags { "LightMode"="PrePassFinal" }
            ZWrite Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepassFinalBack
            #pragma fragment frag_prepassFinalBack
            #pragma multi_compile_prepassfinal
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            sampler2D unity_LightmapInd;
            float4 unity_LightmapST;
            float4 unity_LightmapFade;
            // float4 unity_ShadowFadeCenterAndType;
            #endif

            struct appdata_prepassFinalBack
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_prepassFinalBack
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 projPos : TEXCOORD1;
                float3 shOrLmapUV : TEXCOORD2;
                float4 fade : TEXCOORD3;
            };

            fixed4 _Color;
            sampler2D _BeltTex;
            float4 _MainTex_ST;
            sampler2D _LightBuffer;

            v2f_prepassFinalBack vert_prepassFinalBack(appdata_prepassFinalBack v)
            {
                v2f_prepassFinalBack o;
                UNITY_INITIALIZE_OUTPUT(v2f_prepassFinalBack, o);

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_OFF)
                    o.shOrLmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    float3 worldPos = mul(_Object2World, v.vertex).xyz;
                    o.fade.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
                    o.fade.w = -mul(UNITY_MATRIX_MV, v.vertex).z * (1.0 - unity_ShadowFadeCenterAndType.w);
                #elif defined(LIGHTMAP_ON)
                    o.shOrLmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                    float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                    o.shOrLmapUV = ShadeSH9(half4(worldNormal, 1.0));
                #endif

                return o;
            }

            fixed4 frag_prepassFinalBack(v2f_prepassFinalBack i) : SV_Target
            {
                fixed4 c = tex2D(_BeltTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.projPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                    light.rgb = max(light.rgb, 0.001h);
                #else
                    light.rgb = -log2(max(light.rgb, 0.001h));
                #endif

                #ifdef LIGHTMAP_ON
                    half3 lmFull = tex2D(unity_Lightmap, i.shOrLmapUV.xy).rgb * 2.0;
                    #ifdef DIRLIGHTMAP_OFF
                        half lmFade = sqrt(dot(i.fade, i.fade)) * unity_LightmapFade.z + unity_LightmapFade.w;
                        half3 lmIndirect = tex2D(unity_LightmapInd, i.shOrLmapUV.xy).rgb * 2.0;
                        light.rgb += lerp(lmIndirect, lmFull, saturate(lmFade));
                    #else
                        light.rgb += lmFull;
                    #endif
                #else
                    light.rgb += i.shOrLmapUV;
                #endif

                fixed4 col;
                col.rgb = albedo * light.rgb;
                col.a = 0;
                return col;
            }
            ENDCG
        }

        // =====================================================================
        // FRONT LAYER (overlay) - samples _MainTex, applies UV1-driven vertex
        // offset, always renders on top (ZTest Always)
        // =====================================================================

        Pass
        {
            Name "FORWARDBASE_FRONT"
            Tags { "LightMode"="ForwardBase" }
            ZTest Always

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_baseFront
            #pragma fragment frag_baseFront
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            float4 unity_LightmapST;
            #endif

            struct appdata_baseFront
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_baseFront
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 vLight : TEXCOORD2;
                float2 lmap : TEXCOORD3;
                LIGHTING_COORDS(4,5)
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OffsetX;
            float _OffsetY;

            v2f_baseFront vert_baseFront(appdata_baseFront v)
            {
                v2f_baseFront o;
                UNITY_INITIALIZE_OUTPUT(v2f_baseFront, o);

                float2 xyOffset = v.texcoord1.xy * float2(_OffsetX, _OffsetY);
                v.vertex.xy += xyOffset;

                float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.worldNormal = worldNormal;

                float3 vLight = ShadeSH9(half4(worldNormal, 1.0));
                #ifdef VERTEXLIGHT_ON
                    float3 worldPos = mul(_Object2World, v.vertex).xyz;
                    vLight += Shade4PointLights(
                        unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                        unity_LightColor[0].rgb, unity_LightColor[1].rgb,
                        unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                        unity_4LightAtten0, worldPos, worldNormal);
                #endif
                o.vLight = vLight;

                #ifdef LIGHTMAP_ON
                    o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_baseFront(v2f_baseFront i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                fixed4 col;
                #ifdef LIGHTMAP_ON
                    fixed3 lm = tex2D(unity_Lightmap, i.lmap).rgb * 2.0;
                    col.rgb = albedo * lm;
                #else
                    fixed atten = LIGHT_ATTENUATION(i);
                    fixed3 diff = albedo * _LightColor0.rgb *
                        (max(0, dot(i.worldNormal, _WorldSpaceLightPos0.xyz)) * atten * 2.0);
                    col.rgb = diff + albedo * i.vLight;
                #endif
                col.a = 0;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "FORWARDADD_FRONT"
            Tags { "LightMode"="ForwardAdd" }
            ZTest Always
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_addFront
            #pragma fragment frag_addFront
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata_addFront
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_addFront
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OffsetX;
            float _OffsetY;

            v2f_addFront vert_addFront(appdata_addFront v)
            {
                v2f_addFront o;
                UNITY_INITIALIZE_OUTPUT(v2f_addFront, o);

                float2 xyOffset = v.texcoord1.xy * float2(_OffsetX, _OffsetY);
                v.vertex.xy += xyOffset;

                float3 worldPos = mul(_Object2World, v.vertex).xyz;
                float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.worldNormal = worldNormal;

                #ifdef USING_DIRECTIONAL_LIGHT
                    o.lightDir = _WorldSpaceLightPos0.xyz;
                #else
                    o.lightDir = _WorldSpaceLightPos0.xyz - worldPos;
                #endif

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_addFront(v2f_addFront i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                fixed3 lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 col;
                col.rgb = albedo * _LightColor0.rgb *
                    (max(0, dot(i.worldNormal, lightDir)) * atten * 2.0);
                col.a = 0;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASSBASE_FRONT"
            Tags { "LightMode"="PrePassBase" }
            ZTest Always
            Fog { Mode Off }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepassBaseFront
            #pragma fragment frag_prepassBaseFront
            #include "UnityCG.cginc"

            struct appdata_prepassBaseFront
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_prepassBaseFront
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            float _OffsetX;
            float _OffsetY;

            v2f_prepassBaseFront vert_prepassBaseFront(appdata_prepassBaseFront v)
            {
                v2f_prepassBaseFront o;
                UNITY_INITIALIZE_OUTPUT(v2f_prepassBaseFront, o);

                float2 xyOffset = v.texcoord1.xy * float2(_OffsetX, _OffsetY);
                v.vertex.xy += xyOffset;

                o.worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            fixed4 frag_prepassBaseFront(v2f_prepassBaseFront i) : SV_Target
            {
                fixed4 res;
                res.rgb = i.worldNormal * 0.5 + 0.5;
                res.a = 0;
                return res;
            }
            ENDCG
        }

        Pass
        {
            Name "PREPASSFINAL_FRONT"
            Tags { "LightMode"="PrePassFinal" }
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepassFinalFront
            #pragma fragment frag_prepassFinalFront
            #pragma multi_compile_prepassfinal
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #ifdef LIGHTMAP_ON
            sampler2D unity_Lightmap;
            sampler2D unity_LightmapInd;
            float4 unity_LightmapST;
            float4 unity_LightmapFade;
            // float4 unity_ShadowFadeCenterAndType;
            #endif

            struct appdata_prepassFinalFront
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };

            struct v2f_prepassFinalFront
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 projPos : TEXCOORD1;
                float3 shOrLmapUV : TEXCOORD2;
                float4 fade : TEXCOORD3;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OffsetX;
            float _OffsetY;
            sampler2D _LightBuffer;

            v2f_prepassFinalFront vert_prepassFinalFront(appdata_prepassFinalFront v)
            {
                v2f_prepassFinalFront o;
                UNITY_INITIALIZE_OUTPUT(v2f_prepassFinalFront, o);

                float2 xyOffset = v.texcoord1.xy * float2(_OffsetX, _OffsetY);
                v.vertex.xy += xyOffset;

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_OFF)
                    o.shOrLmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    float3 worldPos = mul(_Object2World, v.vertex).xyz;
                    o.fade.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
                    o.fade.w = -mul(UNITY_MATRIX_MV, v.vertex).z * (1.0 - unity_ShadowFadeCenterAndType.w);
                #elif defined(LIGHTMAP_ON)
                    o.shOrLmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                    float3 worldNormal = mul((float3x3)_Object2World, normalize(v.normal) * unity_Scale.w);
                    o.shOrLmapUV = ShadeSH9(half4(worldNormal, 1.0));
                #endif

                return o;
            }

            fixed4 frag_prepassFinalFront(v2f_prepassFinalFront i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                fixed3 albedo = c.rgb;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.projPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                    light.rgb = max(light.rgb, 0.001h);
                #else
                    light.rgb = -log2(max(light.rgb, 0.001h));
                #endif

                #ifdef LIGHTMAP_ON
                    half3 lmFull = tex2D(unity_Lightmap, i.shOrLmapUV.xy).rgb * 2.0;
                    #ifdef DIRLIGHTMAP_OFF
                        half lmFade = sqrt(dot(i.fade, i.fade)) * unity_LightmapFade.z + unity_LightmapFade.w;
                        half3 lmIndirect = tex2D(unity_LightmapInd, i.shOrLmapUV.xy).rgb * 2.0;
                        light.rgb += lerp(lmIndirect, lmFull, saturate(lmFade));
                    #else
                        light.rgb += lmFull;
                    #endif
                #else
                    light.rgb += i.shOrLmapUV;
                #endif

                fixed4 col;
                col.rgb = albedo * light.rgb;
                col.a = 0;
                return col;
            }
            ENDCG
        }
    }
}