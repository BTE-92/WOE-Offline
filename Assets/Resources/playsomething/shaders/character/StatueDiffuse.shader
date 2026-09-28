Shader "Character/StatueDiffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB)", 2D) = "white" {}
        _GrungeTex ("Grunge (RGB)", 2D) = "white" {}
        _BumpMap ("Bump (RGB) Illumin (A)", 2D) = "bump" {}
        _BaseTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("Scale", Float) = 1
        _Tighten ("Tighten", Range(0.1,0.45)) = 0.3
        _DiffuseAmount ("Diffuse Amount", Range(0,1)) = 0.2
        _GrungeAmount ("Grunge Amount", Range(0.01,2)) = 1
    }

    SubShader { 
        LOD 200
        Tags { "RenderType"="Opaque" }

        // ============================================================
        // PASS 1: FORWARD BASE (Direct Light, SH Ambient, Shadows)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_base
            #pragma fragment frag_base
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GrungeTex;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            sampler2D _BaseTex;
            float4 _Color;
            float _Scale;
            float _Tighten;
            float _DiffuseAmount;
            float _GrungeAmount;

            struct appdata_base_statue {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_base {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;            // xy = MainTex, zw = BumpMap
                float4 worldPos : TEXCOORD1;       // xyz = worldPos, w = lightTangent.x
                float4 worldXTangent : TEXCOORD2;  // xyz = worldXTangent, w = lightTangent.y
                float4 worldYTangent : TEXCOORD3;  // xyz = worldYTangent, w = lightTangent.z
                float3 worldZTangent : TEXCOORD4;
                fixed3 shLight : COLOR0;
                SHADOW_COORDS(5)
            };

            v2f_base vert_base (appdata_base_statue v) {
                v2f_base o;
                UNITY_INITIALIZE_OUTPUT(v2f_base, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Reconstruct Tangent Space transformation matrix
                TANGENT_SPACE_ROTATION;

                // Reconstruct world basis projections in object space
                float3 worldXInObj = unity_ObjectToWorld[0].xyz;
                float3 worldYInObj = unity_ObjectToWorld[1].xyz;
                float3 worldZInObj = unity_ObjectToWorld[2].xyz;

                o.worldXTangent.xyz = mul(rotation, worldXInObj);
                o.worldYTangent.xyz = mul(rotation, worldYInObj);
                o.worldZTangent = mul(rotation, worldZInObj);

                // Tangent-space Light Direction
                float3 lightTangent = mul(rotation, ObjSpaceLightDir(v.vertex));

                // Pack lightTangent into .w components to fit under 8 interpolator limit
                o.worldPos.w = lightTangent.x;
                o.worldXTangent.w = lightTangent.y;
                o.worldYTangent.w = lightTangent.z;

                // Spherical Harmonics ambient evaluated at World Normal
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag_base (v2f_base i) : SV_Target {
                // Unpack lightTangent from .w components
                float3 lightTangent = float3(i.worldPos.w, i.worldXTangent.w, i.worldYTangent.w);

                // 1. Unpack normal in tangent space
                half3 normalTangent = tex2D(_BumpMap, i.uv.zw).rgb * 2.0 - 1.0;

                // 2. Project tangent normal onto world-space tangent basis to get World Normal
                half3 worldNormal;
                worldNormal.x = dot(i.worldXTangent.xyz, normalTangent);
                worldNormal.y = dot(i.worldYTangent.xyz, normalTangent);
                worldNormal.z = dot(i.worldZTangent, normalTangent);
                worldNormal = normalize(worldNormal);

                // 3. Sharp triplanar blending calculations with zero-length guard
                half3 blend = max(abs(worldNormal) - _Tighten, 0.0);
                float totalBlend = blend.x + blend.y + blend.z;
                blend /= max(totalBlend, 0.00001);

                // 4. Sample and interpolate triplanar textures
                float2 uv_xz = i.worldPos.xz / _Scale;
                float2 uv_xy = i.worldPos.xy / _Scale;
                float2 uv_zy = i.worldPos.zy / _Scale;

                half4 cx = tex2D(_BaseTex, uv_zy);
                half4 cz = tex2D(_BaseTex, uv_xy);
                half4 cy_base = tex2D(_BaseTex, uv_xz);
                half4 cy_grunge = tex2D(_GrungeTex, uv_xz);
                
                half4 cy = lerp(cy_base, cy_grunge, _GrungeAmount);
                half4 triplanarCol = cx * blend.xxxx + cy * blend.yyyy + cz * blend.zzzz;

                // 5. Final texture color blending
                half4 mainCol = tex2D(_MainTex, i.uv.xy);
                half4 blendedTex = lerp(triplanarCol, mainCol, _DiffuseAmount);

                half3 albedo = blendedTex.rgb * _Color.rgb;
                half alpha = blendedTex.a * _Color.a;

                // 6. Direct and Ambient Lighting
                float3 lightDir = normalize(lightTangent);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * (NdotL * shadow) + (albedo * i.shLight);
                c.a = alpha;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 2: FORWARD ADD (Pixel Lights)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardAdd" "RenderType"="Opaque" }
            ZWrite Off
            Fog { Color (0,0,0,0) }
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_add
            #pragma fragment frag_add
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GrungeTex;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            sampler2D _BaseTex;
            float4 _Color;
            float _Scale;
            float _Tighten;
            float _DiffuseAmount;
            float _GrungeAmount;

            struct appdata_add_statue {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_add {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;      // xyz = worldPos, w = lightTangent.x
                float4 worldXTangent : TEXCOORD2;  // xyz = worldXTangent, w = lightTangent.y
                float4 worldYTangent : TEXCOORD3;  // xyz = worldYTangent, w = lightTangent.z
                float3 worldZTangent : TEXCOORD4;
                LIGHTING_COORDS(5, 6)
            };

            v2f_add vert_add (appdata_add_statue v) {
                v2f_add o;
                UNITY_INITIALIZE_OUTPUT(v2f_add, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;

                TANGENT_SPACE_ROTATION;

                float3 worldXInObj = unity_ObjectToWorld[0].xyz;
                float3 worldYInObj = unity_ObjectToWorld[1].xyz;
                float3 worldZInObj = unity_ObjectToWorld[2].xyz;

                o.worldXTangent.xyz = mul(rotation, worldXInObj);
                o.worldYTangent.xyz = mul(rotation, worldYInObj);
                o.worldZTangent = mul(rotation, worldZInObj);

                // Tangent-space Light Direction
                float3 lightTangent = mul(rotation, ObjSpaceLightDir(v.vertex));

                // Pack lightTangent into .w components
                o.worldPos.w = lightTangent.x;
                o.worldXTangent.w = lightTangent.y;
                o.worldYTangent.w = lightTangent.z;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag_add (v2f_add i) : SV_Target {
                float3 lightTangent = float3(i.worldPos.w, i.worldXTangent.w, i.worldYTangent.w);

                half3 normalTangent = tex2D(_BumpMap, i.uv.zw).rgb * 2.0 - 1.0;

                half3 worldNormal;
                worldNormal.x = dot(i.worldXTangent.xyz, normalTangent);
                worldNormal.y = dot(i.worldYTangent.xyz, normalTangent);
                worldNormal.z = dot(i.worldZTangent, normalTangent);
                worldNormal = normalize(worldNormal);

                half3 blend = max(abs(worldNormal) - _Tighten, 0.0);
                float totalBlend = blend.x + blend.y + blend.z;
                blend /= max(totalBlend, 0.00001);

                float2 uv_xz = i.worldPos.xz / _Scale;
                float2 uv_xy = i.worldPos.xy / _Scale;
                float2 uv_zy = i.worldPos.zy / _Scale;

                half4 cx = tex2D(_BaseTex, uv_zy);
                half4 cz = tex2D(_BaseTex, uv_xy);
                half4 cy_base = tex2D(_BaseTex, uv_xz);
                half4 cy_grunge = tex2D(_GrungeTex, uv_xz);
                
                half4 cy = lerp(cy_base, cy_grunge, _GrungeAmount);
                half4 triplanarCol = cx * blend.xxxx + cy * blend.yyyy + cz * blend.zzzz;

                half4 mainCol = tex2D(_MainTex, i.uv.xy);
                half4 blendedTex = lerp(triplanarCol, mainCol, _DiffuseAmount);

                half3 albedo = blendedTex.rgb * _Color.rgb;
                half alpha = blendedTex.a * _Color.a;

                float3 lightDir = normalize(lightTangent);
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * (NdotL * atten);
                c.a = alpha;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PASS 3: DEFERRED PREPASS BASE (Normal Buffering)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassBase" "RenderType"="Opaque" }
            Fog { Mode Off }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepass_base
            #pragma fragment frag_prepass_base

            #include "UnityCG.cginc"

            sampler2D _BumpMap;
            float4 _BumpMap_ST;

            struct appdata_prepass {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_prepass_base {
                float4 pos : SV_POSITION;
                float2 uvBump : TEXCOORD0;
                float3 worldTangent : TEXCOORD1;
                float3 worldBinormal : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            v2f_prepass_base vert_prepass_base (appdata_prepass v) {
                v2f_prepass_base o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvBump = TRANSFORM_TEX(v.texcoord, _BumpMap);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;

                return o;
            }

            fixed4 frag_prepass_base (v2f_prepass_base i) : SV_Target {
                half3 normalTangent = tex2D(_BumpMap, i.uvBump).rgb * 2.0 - 1.0;
                half3 worldN = normalTangent.x * i.worldTangent + normalTangent.y * i.worldBinormal + normalTangent.z * i.worldNormal;
                worldN = normalize(worldN);

                fixed4 res;
                res.xyz = worldN * 0.5 + 0.5;
                res.w = 0.0;
                return res;
            }
            ENDCG
        }

        // ============================================================
        // PASS 4: DEFERRED PREPASS FINAL (Lighting Combine)
        // ============================================================
        Pass {
            Name "PREPASS"
            Tags { "LightMode"="PrePassFinal" "RenderType"="Opaque" }
            ZWrite Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_prepass_final
            #pragma fragment frag_prepass_final
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
            #pragma multi_compile HDR_LIGHT_PREPASS_OFF HDR_LIGHT_PREPASS_ON

            #include "UnityCG.cginc"

            sampler2D _LightBuffer;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GrungeTex;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            sampler2D _BaseTex;
            float4 _Color;
            float _Scale;
            float _Tighten;
            float _DiffuseAmount;
            float _GrungeAmount;

            struct appdata_prepass {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f_prepass_final {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;          // xy = MainTex, zw = BumpMap
                float3 worldPos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 worldXTangent : TEXCOORD3;
                float3 worldYTangent : TEXCOORD4;
                float3 worldZTangent : TEXCOORD5;
                fixed3 shLight : COLOR0;
            };

            v2f_prepass_final vert_prepass_final (appdata_prepass v) {
                v2f_prepass_final o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.screenPos = ComputeScreenPos(o.pos);

                TANGENT_SPACE_ROTATION;

                float3 worldXInObj = unity_ObjectToWorld[0].xyz;
                float3 worldYInObj = unity_ObjectToWorld[1].xyz;
                float3 worldZInObj = unity_ObjectToWorld[2].xyz;

                o.worldXTangent = mul(rotation, worldXInObj);
                o.worldYTangent = mul(rotation, worldYInObj);
                o.worldZTangent = mul(rotation, worldZInObj);

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));

                return o;
            }

            fixed4 frag_prepass_final (v2f_prepass_final i) : SV_Target {
                half3 normalTangent = tex2D(_BumpMap, i.uv.zw).rgb * 2.0 - 1.0;

                half3 worldNormal;
                worldNormal.x = dot(i.worldXTangent, normalTangent);
                worldNormal.y = dot(i.worldYTangent, normalTangent);
                worldNormal.z = dot(i.worldZTangent, normalTangent);
                worldNormal = normalize(worldNormal);

                half3 blend = max(abs(worldNormal) - _Tighten, 0.0);
                float totalBlend = blend.x + blend.y + blend.z;
                blend /= max(totalBlend, 0.00001);

                float2 uv_xz = i.worldPos.xz / _Scale;
                float2 uv_xy = i.worldPos.xy / _Scale;
                float2 uv_zy = i.worldPos.zy / _Scale;

                half4 cx = tex2D(_BaseTex, uv_zy);
                half4 cz = tex2D(_BaseTex, uv_xy);
                half4 cy_base = tex2D(_BaseTex, uv_xz);
                half4 cy_grunge = tex2D(_GrungeTex, uv_xz);
                
                half4 cy = lerp(cy_base, cy_grunge, _GrungeAmount);
                half4 triplanarCol = cx * blend.xxxx + cy * blend.yyyy + cz * blend.zzzz;

                half4 mainCol = tex2D(_MainTex, i.uv.xy);
                half4 blendedTex = lerp(triplanarCol, mainCol, _DiffuseAmount);

                half3 albedo = blendedTex.rgb * _Color.rgb;
                half alpha = blendedTex.a * _Color.a;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                    light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                    light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                light.rgb += i.shLight;

                fixed4 c;
                c.rgb = albedo * light.rgb;
                c.a = alpha;
                return c;
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}