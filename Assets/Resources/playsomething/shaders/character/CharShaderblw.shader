Shader "Character/CharShader-bumped-lambertwrap" {
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
        // FORWARD BASE PASS (Direct Light, SH Ambient, Shadows)
        // ============================================================
        Pass {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

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
                float4 uv : TEXCOORD0;       // xy = _MainTex, zw = _BumpMap
                float3 lightDirTangent : TEXCOORD1;
                #ifndef LIGHTMAP_ON
                fixed3 shLight : COLOR0;
                #endif
                SHADOW_COORDS(2)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                // Build tangent space rotation matrix to transform object-space light dir
                TANGENT_SPACE_ROTATION;
                o.lightDirTangent = mul(rotation, ObjSpaceLightDir(v.vertex));

                #ifndef LIGHTMAP_ON
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
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
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                // 5. Lighting Math (Wrapped Half-Lambert with Shadows & SH Ambient)
                float3 lightDir = normalize(i.lightDirTangent);
                fixed shadow = SHADOW_ATTENUATION(i);

                // Half-lambert wrapping formulation: (N·L * 0.5 + 0.5) * shadow
                half NdotLWrap = (dot(normalTangent, lightDir) * 0.5 + 0.5) * shadow;

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * NdotLWrap;

                #ifndef LIGHTMAP_ON
                c.rgb += albedo * i.shLight;
                #endif

                c.a = 0.0;
                return c;
            }
            ENDCG
        }

        // ============================================================
        // FORWARD ADD PASS (Additive Pixel Lights)
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
                LIGHTING_COORDS(2, 3)
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

                TANGENT_SPACE_ROTATION;
                o.lightDirTangent = mul(rotation, ObjSpaceLightDir(v.vertex));

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
                half3 normalTangent = UnpackNormal(tex2D(_BumpMap, i.uv.zw));

                float3 lightDir = normalize(i.lightDirTangent);
                fixed atten = LIGHT_ATTENUATION(i);

                half NdotLWrap = (dot(normalTangent, lightDir) * 0.5 + 0.5) * atten;

                fixed4 c;
                c.rgb = (albedo * _LightColor0.rgb) * NdotLWrap;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}