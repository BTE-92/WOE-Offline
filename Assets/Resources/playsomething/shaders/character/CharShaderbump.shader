#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Character/CharShader-bumped" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("MaskTex (RGBA)", 2D) = "black" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _EyeColor ("Eye Color", Color) = (0.5,0.5,0.5,1)
        _SkinColor ("Skin Color", Color) = (0.5,0.5,0.5,1)
        _HairColor ("Hair Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ============================================================
        // FORWARD BASE PASS (Direct Light, Specular, SH Ambient, Shadows)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            fixed4 _EyeColor;
            fixed4 _SkinColor;
            fixed4 _HairColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;              // xy = _MainTex, zw = _BumpMap
                float3 lightDirTangent : TEXCOORD1;  // Tangent-space light dir
                #ifndef LIGHTMAP_ON
                fixed3 shLight : COLOR0;
                #endif
                float3 viewDirTangent : TEXCOORD3;   // Tangent-space view dir
                SHADOW_COORDS(4)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                TANGENT_SPACE_ROTATION;

                // Transform light direction to tangent space
                float3 objLightDir = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
                o.lightDirTangent = mul(rotation, objLightDir);

                // Transform view direction to tangent space
                float3 objViewDir = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz * 1.0 - v.vertex.xyz;
                o.viewDirTangent = mul(rotation, objViewDir);

                #ifndef LIGHTMAP_ON
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal * 1.0));
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                #endif

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. Samples base and mask colors
                half4 basecol = tex2D(_MainTex, i.uv.xy);
                half4 maskcol = tex2D(_MaskTex, i.uv.xy);

                // 2. Eyes Mask Blend (Red channel)
                if (maskcol.r > 0.0) {
                    half luminance = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                    half3 eyeBlend = luminance * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, eyeBlend, maskcol.r);
                }

                // 3. Skin Mask Blend (Green channel)
                if (maskcol.g > 0.0) {
                    half3 skinBlend = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, skinBlend, maskcol.g);
                }

                // 4. Hair Mask Blend (Blue channel)
                if (maskcol.b > 0.0) {
                    half3 hairBlend = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, hairBlend, maskcol.b);
                }

                half3 albedo = basecol.rgb;
                half specMask = basecol.a;
                half gloss = basecol.a;

                // 5. Tangent space vectors setup
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                float3 lightDir = normalize(i.lightDirTangent);
                float3 viewDir = normalize(i.viewDirTangent);
                float3 halfDir = normalize(lightDir + viewDir);

                // 6. Direct light calculations
                float NdotL = max(0.0, dot(normalTangent, lightDir));
                float NdotH = max(0.0, dot(normalTangent, halfDir));
                fixed shadow = SHADOW_ATTENUATION(i);

                float specExp = gloss * 128.0;
                float specTerm = pow(NdotH, specExp) * specMask;

                fixed4 c;
                // Direct Light (modulated by shadow * 2.0)
                c.rgb = ((albedo * _LightColor0.rgb) * NdotL + (_LightColor0.rgb * _SpecColor.rgb) * specTerm) * (shadow * 2.0);

                #ifndef LIGHTMAP_ON
                // Add spherical harmonics indirect diffuse
                c.rgb += albedo * i.shLight;
                #endif

                c.a = _LightColor0.a * _SpecColor.a * specTerm * shadow;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS (Additive Lights)
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
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            fixed4 _EyeColor;
            fixed4 _SkinColor;
            fixed4 _HairColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 lightDirTangent : TEXCOORD1;
                float3 viewDirTangent : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                TANGENT_SPACE_ROTATION;

                // Handle local position offset vector conversion for point/spot lights
                float3 objLightVec = mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz;
                if (_WorldSpaceLightPos0.w > 0.0) {
                    objLightVec = objLightVec * 1.0 - v.vertex.xyz;
                }
                o.lightDirTangent = mul(rotation, objLightVec);

                // View direction transformation
                float3 objViewDir = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz * 1.0 - v.vertex.xyz;
                o.viewDirTangent = mul(rotation, objViewDir);

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 basecol = tex2D(_MainTex, i.uv.xy);
                half4 maskcol = tex2D(_MaskTex, i.uv.xy);

                if (maskcol.r > 0.0) {
                    half luminance = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                    half3 eyeBlend = luminance * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, eyeBlend, maskcol.r);
                }
                if (maskcol.g > 0.0) {
                    half3 skinBlend = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, skinBlend, maskcol.g);
                }
                if (maskcol.b > 0.0) {
                    half3 hairBlend = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, hairBlend, maskcol.b);
                }

                half3 albedo = basecol.rgb;
                half specMask = basecol.a;
                half gloss = basecol.a;

                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                float3 lightDir = normalize(i.lightDirTangent);
                float3 viewDir = normalize(i.viewDirTangent);
                float3 halfDir = normalize(lightDir + viewDir);

                float NdotL = max(0.0, dot(normalTangent, lightDir));
                float NdotH = max(0.0, dot(normalTangent, halfDir));
                fixed atten = LIGHT_ATTENUATION(i);

                float specExp = gloss * 128.0;
                float specTerm = pow(NdotH, specExp) * specMask;

                fixed4 c;
                c.rgb = ((albedo * _LightColor0.rgb) * NdotL + (_LightColor0.rgb * _SpecColor.rgb) * specTerm) * (atten * 2.0);
                c.a = (_LightColor0.a * _SpecColor.a * specTerm) * atten;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // PREPASS BASE (Deferred Lighting - Normals & Specular Power)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 tspace0 : TEXCOORD1; // World Tangent
                float3 tspace1 : TEXCOORD2; // World Binormal
                float3 tspace2 : TEXCOORD3; // World Normal
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal * 1.0));
                float3 worldTangent = normalize(mul((float3x3)unity_ObjectToWorld, v.tangent.xyz));
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                o.tspace0 = worldTangent;
                o.tspace1 = worldBinormal;
                o.tspace2 = worldNormal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 basecol = tex2D(_MainTex, i.uv.xy);
                half gloss = basecol.a;

                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                half3 worldNormal = normalize(normalTangent.x * i.tspace0 + normalTangent.y * i.tspace1 + normalTangent.z * i.tspace2);

                fixed4 res;
                res.xyz = worldNormal * 0.5 + 0.5;
                res.w = gloss;
                return res;
            }
            ENDCG
        }

        // ============================================================
        // PREPASS FINAL (Deferred Lighting - Albedo & Light Accumulation)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            sampler2D _LightBuffer;
            fixed4 _EyeColor;
            fixed4 _SkinColor;
            fixed4 _HairColor;
            fixed4 _SpecColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                fixed3 shLight : COLOR0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);

                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal * 1.0));
                o.shLight = ShadeSH9(float4(worldNormal, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                half4 basecol = tex2D(_MainTex, i.uv);
                half4 maskcol = tex2D(_MaskTex, i.uv);

                if (maskcol.r > 0.0) {
                    half luminance = dot(basecol.rgb, half3(0.3, 0.59, 0.11));
                    half3 eyeBlend = luminance * 2.0 * _EyeColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, eyeBlend, maskcol.r);
                }
                if (maskcol.g > 0.0) {
                    half3 skinBlend = basecol.rgb * 2.0 * _SkinColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, skinBlend, maskcol.g);
                }
                if (maskcol.b > 0.0) {
                    half3 hairBlend = basecol.rgb * 2.0 * _HairColor.rgb;
                    basecol.rgb = lerp(basecol.rgb, hairBlend, maskcol.b);
                }

                half3 albedo = basecol.rgb;
                half gloss = basecol.a;

                half4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screenPos));

                #ifdef HDR_LIGHT_PREPASS_ON
                light = max(light, half4(0.001, 0.001, 0.001, 0.001));
                #else
                light = -log2(max(light, half4(0.001, 0.001, 0.001, 0.001)));
                #endif

                // Add spherical harmonics ambient
                light.rgb += i.shLight;

                // Evaluates specular parameters matching LightBuffer layout structure
                half specPower = light.a * gloss;

                fixed4 c;
                c.rgb = albedo * light.rgb + (light.rgb * _SpecColor.rgb) * specPower;
                c.a = specPower * _SpecColor.a;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}